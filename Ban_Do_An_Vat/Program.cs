using Ban_Do_An_Vat.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ban_Do_An_Vat.Services;
using Ban_Do_An_Vat.Models;

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

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

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
