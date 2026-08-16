using Ban_Do_An_Vat.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ban_Do_An_Vat.Services;
using Ban_Do_An_Vat.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Ban_Do_An_Vat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── DATABASE CONFIGURATION ────────────────────────────────────────────
            // Dual-database: PostgreSQL (Neon) trên Render, SQL Server trên local.
            // Thêm 07/08/2026 để hỗ trợ deploy lên Render + Neon.
            //
            // Cách hoạt động:
            //   - Nếu tồn tại biến môi trường DATABASE_URL (Render/Neon) → dùng PostgreSQL
            //   - Ngược lại → dùng DefaultConnection trong appsettings.json (SQL Server, local)
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrEmpty(databaseUrl))
            {
                // Fix 07/08/2026: PostgreSQL (Npgsql) mặc định yêu cầu DateTime Kind=UTC.
                // Khi form HTML submit datetime-local → model binder tạo Kind=Unspecified.
                // AppContext.SetSwitch này bật "legacy mode": chấp nhận mọi Kind (Local/Unspecified/Utc)
                // mà không throw exception — tương thích ngược với toàn bộ code hiện tại.
                // Chỉ kích hoạt khi dùng PostgreSQL (Render), không ảnh hưởng SQL Server (local).
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                // PostgreSQL cho Render/Neon production — dùng ApplicationDbContextPostgres
                // có migrations riêng trong Data/MigrationsPostgres/
                // Fix 07/08/2026: Neon cấp URI format (postgresql://...) nhưng Npgsql cần
                // key-value format → dùng ConvertPostgresUrlToNpgsql() để convert
                var npgsqlConnStr = ConvertPostgresUrlToNpgsql(databaseUrl);
                builder.Services.AddDbContext<ApplicationDbContext, ApplicationDbContextPostgres>(options =>
                    options.UseNpgsql(npgsqlConnStr));
            }
            else
            {
                // SQL Server cho local development — giữ nguyên như cũ
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            // ─────────────────────────────────────────────────────────────────────

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // [SEC-04] Mật khẩu yêu cầu tối thiểu: 8 ký tự, có số
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                // [SEC-04] Khóa tài khoản sau 5 lần nhập sai trong 15 phút
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });
            
            // Payment Integration Services
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IMomoService, MomoService>();

            // AI Chatbot Service
            builder.Services.AddHttpClient<IGeminiService, GeminiService>();

            // [SEC-05] Rate Limiting — ngăn brute-force đăng nhập và lạm dụng chatbot AI
            builder.Services.AddRateLimiter(options =>
            {
                // Login: tối đa 10 lần / 15 phút mỗi IP
                options.AddFixedWindowLimiter("login", cfg =>
                {
                    cfg.Window = TimeSpan.FromMinutes(15);
                    cfg.PermitLimit = 10;
                    cfg.QueueLimit = 0;
                    cfg.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
                // Chatbot: tối đa 20 request / phút mỗi IP (bảo vệ Gemini API cost)
                options.AddFixedWindowLimiter("chatbot", cfg =>
                {
                    cfg.Window = TimeSpan.FromMinutes(1);
                    cfg.PermitLimit = 20;
                    cfg.QueueLimit = 0;
                    cfg.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
                options.RejectionStatusCode = 429;
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // ── AUTO-MIGRATE trên Production (Render) ────────────────────────────
            // Tự động chạy migrations khi deploy lên Render, an toàn vì chỉ kích hoạt
            // khi DATABASE_URL tồn tại. Local dev vẫn dùng lệnh thủ công.
            // Thêm 07/08/2026
            if (!string.IsNullOrEmpty(databaseUrl))
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }
            // ─────────────────────────────────────────────────────────────────────

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Fix 16/08/2026: 404/403 trước đây trả về body rỗng (trang trắng).
            // Re-execute về /Home/Error/{0} để hiển thị trang lỗi thân thiện
            // (ví dụ xem chi tiết sản phẩm/đơn hàng không tồn tại), giữ nguyên URL gốc.
            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // [SEC-08] Security headers — ngăn clickjacking, MIME sniffing, referrer leak
            app.Use(async (ctx, next) =>
            {
                ctx.Response.Headers["X-Frame-Options"] = "DENY";
                ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
                ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                ctx.Response.Headers["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";
                await next();
            });

            app.UseRouting();
            // [SEC-05] Rate limiting middleware — phải được gọn trước UseAuthentication
            app.UseRateLimiter();
            app.UseSession();

            // [SEC-07] UseAuthentication() phải được gọi tường minh trước UseAuthorization()
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }

        // ── HELPER: Convert PostgreSQL URI → Npgsql connection string ────────
        // Neon cấp connection string dạng URI:
        //   postgresql://user:pass@host/db?sslmode=require&channel_binding=require
        // Npgsql cần dạng key-value:
        //   Host=...;Database=...;Username=...;Password=...;SSL Mode=Require
        // Thêm 07/08/2026 để fix lỗi NpgsqlConnectionStringBuilder không nhận URI
        private static string ConvertPostgresUrlToNpgsql(string databaseUrl)
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');
            var username = Uri.UnescapeDataString(userInfo[0]);
            var password = Uri.UnescapeDataString(userInfo[1]);
            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 5432;
            var database = uri.AbsolutePath.TrimStart('/');

            return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        }
        // ─────────────────────────────────────────────────────────────────────
    }
}
