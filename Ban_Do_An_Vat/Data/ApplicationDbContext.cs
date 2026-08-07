using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Ban_Do_An_Vat.Models;
using System;

namespace Ban_Do_An_Vat.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Snack> Snacks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Combo -> ComboItem (cascade delete)
            builder.Entity<ComboItem>()
                .HasOne(ci => ci.Combo)
                .WithMany(c => c.ComboItems)
                .HasForeignKey(ci => ci.ComboId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ComboItem>()
                .HasOne(ci => ci.Snack)
                .WithMany()
                .HasForeignKey(ci => ci.SnackId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Roles
            var adminRoleId = "1";
            var customerRoleId = "2";
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = customerRoleId, Name = "Customer", NormalizedName = "CUSTOMER" }
            );

            // Seed Admin User
            var adminUserId = "1";
            var hasher = new PasswordHasher<ApplicationUser>();
            builder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = adminUserId,
                    UserName = "admin@bandoanvat.com",
                    NormalizedUserName = "ADMIN@BANDOANVAT.COM",
                    Email = "admin@bandoanvat.com",
                    NormalizedEmail = "ADMIN@BANDOANVAT.COM",
                    EmailConfirmed = true,
                    FullName = "Hệ thống Munchies",
                    Address = "123 Đường ăn vặt, Sài Gòn",
                    PasswordHash = hasher.HashPassword(null!, "Admin@123"),
                    SecurityStamp = string.Empty
                }
            );

            // Seed User Role
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );

            // Configure Seed Data
            builder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Bánh Tráng & Khô",
                    Description = "Các loại bánh tráng trộn, bánh tráng cuộn, khô bò, khô gà lá chanh đậm đà hương vị Việt",
                    ImageUrl = "https://images.unsplash.com/photo-1599599810769-bcde5a160d32?q=80&w=600&auto=format&fit=crop"
                },
                new Category
                {
                    Id = 2,
                    Name = "Hạt & Trái Cây Sấy",
                    Description = "Hạt điều rang muối, hạt dẻ, mít sấy, trái cây sấy giòn tự nhiên thơm ngon, bổ dưỡng",
                    ImageUrl = "https://images.unsplash.com/photo-1596560548464-f010689b7f1e?q=80&w=600&auto=format&fit=crop"
                },
                new Category
                {
                    Id = 3,
                    Name = "Bánh & Kẹo Ngọt",
                    Description = "Bánh gấu nhân kem, kẹo dẻo trái cây, bánh quy ngọt ngào ăn vặt vui miệng",
                    ImageUrl = "https://images.unsplash.com/photo-1505252585461-04db1ebb846d?q=80&w=600&auto=format&fit=crop"
                },
                new Category
                {
                    Id = 4,
                    Name = "Ăn Vặt Hiện Đại",
                    Description = "Rong biển cháy tỏi, khoai tây lắc phô mai, các món ăn vặt trào lưu mới",
                    ImageUrl = "https://images.unsplash.com/photo-1613919113640-25732ec5e61f?q=80&w=600&auto=format&fit=crop"
                }
            );

            builder.Entity<Snack>().HasData(
                new Snack
                {
                    Id = 1,
                    CategoryId = 1,
                    Name = "Bánh Tráng Trộn Sài Gòn",
                    Description = "Bánh tráng sợi dai mềm trộn cùng sa tế tôm cay nồng, khô bò sợi, tép khô rang, hành phi thơm lừng, xoài xanh bào và rau răm tươi mát. Hương vị đường phố đích thực.",
                    Price = 25000,
                    StockQuantity = 100,
                    IsAvailable = true,
                    Rating = 4.8,
                    Weight = "150g",
                    Ingredients = "Bánh tráng, sa tế, khô bò, tép rang, xoài xanh, rau răm, nước sốt tắc",
                    ImageUrl = "https://images.unsplash.com/photo-1626132647523-66f5bf380027?q=80&w=600&auto=format&fit=crop"
                },
                new Snack
                {
                    Id = 2,
                    CategoryId = 1,
                    Name = "Khô Bò Xé Sợi Cay Tè",
                    Description = "Thịt bò tươi ngon chọn lọc được tẩm ướp đậm đà với sả băm, tỏi, ớt hiểm cay nồng và mật ong tự nhiên. Sấy khô vừa phải giữ độ mềm ngọt của từng sớ thịt.",
                    Price = 89000,
                    StockQuantity = 50,
                    IsAvailable = true,
                    Rating = 4.9,
                    Weight = "180g",
                    Ingredients = "Thịt bò tươi, sả, tỏi, ớt hiểm, mật ong, ngũ vị hương",
                    ImageUrl = "https://images.unsplash.com/photo-1529042410759-befb1204b468?q=80&w=600&auto=format&fit=crop"
                },
                new Snack
                {
                    Id = 3,
                    CategoryId = 1,
                    Name = "Khô Gà Lá Chanh Giòn Cay",
                    Description = "Ức gà xé tơi giòn rụm kết hợp với lá chanh tươi thái sợi sấy thơm, tỏi phi vàng và ớt khô nguyên quả. Vị mặn ngọt hài hòa, cay thơm dịu nhẹ khó cưỡng.",
                    Price = 45000,
                    StockQuantity = 150,
                    IsAvailable = true,
                    Rating = 4.7,
                    Weight = "150g",
                    Ingredients = "Thịt gà, lá chanh, tỏi phi, ớt khô, gia vị, nước mắm",
                    ImageUrl = "https://images.unsplash.com/photo-1608039755401-742074f0548d?q=80&w=600&auto=format&fit=crop"
                },
                new Snack
                {
                    Id = 4,
                    CategoryId = 1,
                    Name = "Cơm Cháy Siêu Ruốc (Chà Bông)",
                    Description = "Cơm cháy đáy nồi giòn tan rụm lót lớp chà bông heo (ruốc) dày đặc, rưới thêm nước sốt mắm ớt hành kẹo kẹo thơm lừng, béo ngậy.",
                    Price = 39000,
                    StockQuantity = 80,
                    IsAvailable = true,
                    Rating = 4.6,
                    Weight = "250g",
                    Ingredients = "Nếp thơm, chà bông heo sạch, nước sốt mắm ớt, hành lá",
                    ImageUrl = "https://images.unsplash.com/photo-1568254183919-78a4f43a2877?q=80&w=600&auto=format&fit=crop"
                },
                new Snack
                {
                    Id = 5,
                    CategoryId = 2,
                    Name = "Hạt Điều Vỏ Lụa Rang Muối Bình Phước",
                    Description = "Hạt điều loại A hạt to tròn béo bùi nguyên hạt, rang củi thủ công cùng muối tinh giữ nguyên lớp vỏ lụa giúp giữ vị ngọt thanh tự nhiên và độ giòn tan lâu hơn.",
                    Price = 95000,
                    StockQuantity = 60,
                    IsAvailable = true,
                    Rating = 4.8,
                    Weight = "250g",
                    Ingredients = "Hạt điều vỏ lụa Bình Phước, muối tinh 1%",
                    ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?q=80&w=600&auto=format&fit=crop"
                },
                new Snack
                {
                    Id = 6,
                    CategoryId = 2,
                    Name = "Mít Sấy Giòn Xuất Khẩu",
                    Description = "Mít chín cây tươi ngon được sấy nhiệt hiện đại giúp giữ nguyên màu vàng óng, mùi thơm đặc trưng và vị ngọt đậm tự nhiên. Giòn rụm không bị gắt dầu.",
                    Price = 42000,
                    StockQuantity = 120,
                    IsAvailable = true,
                    Rating = 4.5,
                    Weight = "150g",
                    Ingredients = "Mít tươi, dầu cọ thực vật sấy",
                    ImageUrl = "https://images.unsplash.com/photo-1600850756094-8ab05e2ed88a?q=80&w=600&auto=format&fit=crop"
                },
                new Snack
                {
                    Id = 7,
                    CategoryId = 3,
                    Name = "Bánh Gấu Nhân Kem Sữa Béo",
                    Description = "Vỏ bánh giòn xốp tạo hình chú gấu dễ thương ôm trọn nhân kem sữa béo ngậy ngọt ngào bên trong. Món ăn gắn liền với tuổi thơ bao thế hệ.",
                    Price = 30000,
                    StockQuantity = 200,
                    IsAvailable = true,
                    Rating = 4.7,
                    Weight = "200g",
                    Ingredients = "Bột mì, sữa bột, đường cát, bơ thực vật, hương sữa tổng hợp",
                    ImageUrl = "https://images.unsplash.com/photo-1558961309-dbdf7177e419?q=80&w=600&auto=format&fit=crop"
                },
                new Snack
                {
                    Id = 8,
                    CategoryId = 4,
                    Name = "Rong Biển Cháy Tỏi Giòn Tan",
                    Description = "Lá rong biển khô tẩm gia vị muối ớt cay mặn, sấy giòn rụm rồi phi thơm cùng tỏi băm ngập dầu vàng giòn. Thích hợp cho người ăn kiêng thanh đạm.",
                    Price = 35000,
                    StockQuantity = 90,
                    IsAvailable = true,
                    Rating = 4.8,
                    Weight = "90g",
                    Ingredients = "Rong biển nhập khẩu, tỏi phi giòn, muối, ớt, đường",
                    ImageUrl = "https://images.unsplash.com/photo-1607349913338-fca6f7fc42d0?q=80&w=600&auto=format&fit=crop"
                }
            );
        }
    }
}
