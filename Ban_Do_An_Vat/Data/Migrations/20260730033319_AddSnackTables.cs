using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ban_Do_An_Vat.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSnackTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    OrderNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    Weight = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ingredients = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Snacks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    SnackId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Snacks_SnackId",
                        column: x => x.SnackId,
                        principalTable: "Snacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "Các loại bánh tráng trộn, bánh tráng cuộn, khô bò, khô gà lá chanh đậm đà hương vị Việt", "https://images.unsplash.com/photo-1599599810769-bcde5a160d32?q=80&w=600&auto=format&fit=crop", "Bánh Tráng & Khô" },
                    { 2, "Hạt điều rang muối, hạt dẻ, mít sấy, trái cây sấy giòn tự nhiên thơm ngon, bổ dưỡng", "https://images.unsplash.com/photo-1596560548464-f010689b7f1e?q=80&w=600&auto=format&fit=crop", "Hạt & Trái Cây Sấy" },
                    { 3, "Bánh gấu nhân kem, kẹo dẻo trái cây, bánh quy ngọt ngào ăn vặt vui miệng", "https://images.unsplash.com/photo-1505252585461-04db1ebb846d?q=80&w=600&auto=format&fit=crop", "Bánh & Kẹo Ngọt" },
                    { 4, "Rong biển cháy tỏi, khoai tây lắc phô mai, các món ăn vặt trào lưu mới", "https://images.unsplash.com/photo-1613919113640-25732ec5e61f?q=80&w=600&auto=format&fit=crop", "Ăn Vặt Hiện Đại" }
                });

            migrationBuilder.InsertData(
                table: "Snacks",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Ingredients", "IsAvailable", "Name", "Price", "Rating", "StockQuantity", "Weight" },
                values: new object[,]
                {
                    { 1, 1, "Bánh tráng sợi dai mềm trộn cùng sa tế tôm cay nồng, khô bò sợi, tép khô rang, hành phi thơm lừng, xoài xanh bào và rau răm tươi mát. Hương vị đường phố đích thực.", "https://images.unsplash.com/photo-1626132647523-66f5bf380027?q=80&w=600&auto=format&fit=crop", "Bánh tráng, sa tế, khô bò, tép rang, xoài xanh, rau răm, nước sốt tắc", true, "Bánh Tráng Trộn Sài Gòn", 25000m, 4.7999999999999998, 100, "150g" },
                    { 2, 1, "Thịt bò tươi ngon chọn lọc được tẩm ướp đậm đà với sả băm, tỏi, ớt hiểm cay nồng và mật ong tự nhiên. Sấy khô vừa phải giữ độ mềm ngọt của từng sớ thịt.", "https://images.unsplash.com/photo-1529042410759-befb1204b468?q=80&w=600&auto=format&fit=crop", "Thịt bò tươi, sả, tỏi, ớt hiểm, mật ong, ngũ vị hương", true, "Khô Bò Xé Sợi Cay Tè", 89000m, 4.9000000000000004, 50, "180g" },
                    { 3, 1, "Ức gà xé tơi giòn rụm kết hợp với lá chanh tươi thái sợi sấy thơm, tỏi phi vàng và ớt khô nguyên quả. Vị mặn ngọt hài hòa, cay thơm dịu nhẹ khó cưỡng.", "https://images.unsplash.com/photo-1608039755401-742074f0548d?q=80&w=600&auto=format&fit=crop", "Thịt gà, lá chanh, tỏi phi, ớt khô, gia vị, nước mắm", true, "Khô Gà Lá Chanh Giòn Cay", 45000m, 4.7000000000000002, 150, "150g" },
                    { 4, 1, "Cơm cháy đáy nồi giòn tan rụm lót lớp chà bông heo (ruốc) dày đặc, rưới thêm nước sốt mắm ớt hành kẹo kẹo thơm lừng, béo ngậy.", "https://images.unsplash.com/photo-1568254183919-78a4f43a2877?q=80&w=600&auto=format&fit=crop", "Nếp thơm, chà bông heo sạch, nước sốt mắm ớt, hành lá", true, "Cơm Cháy Siêu Ruốc (Chà Bông)", 39000m, 4.5999999999999996, 80, "250g" },
                    { 5, 2, "Hạt điều loại A hạt to tròn béo bùi nguyên hạt, rang củi thủ công cùng muối tinh giữ nguyên lớp vỏ lụa giúp giữ vị ngọt thanh tự nhiên và độ giòn tan lâu hơn.", "https://images.unsplash.com/photo-1509440159596-0249088772ff?q=80&w=600&auto=format&fit=crop", "Hạt điều vỏ lụa Bình Phước, muối tinh 1%", true, "Hạt Điều Vỏ Lụa Rang Muối Bình Phước", 95000m, 4.7999999999999998, 60, "250g" },
                    { 6, 2, "Mít chín cây tươi ngon được sấy nhiệt hiện đại giúp giữ nguyên màu vàng óng, mùi thơm đặc trưng và vị ngọt đậm tự nhiên. Giòn rụm không bị gắt dầu.", "https://images.unsplash.com/photo-1600850756094-8ab05e2ed88a?q=80&w=600&auto=format&fit=crop", "Mít tươi, dầu cọ thực vật sấy", true, "Mít Sấy Giòn Xuất Khẩu", 42000m, 4.5, 120, "150g" },
                    { 7, 3, "Vỏ bánh giòn xốp tạo hình chú gấu dễ thương ôm trọn nhân kem sữa béo ngậy ngọt ngào bên trong. Món ăn gắn liền với tuổi thơ bao thế hệ.", "https://images.unsplash.com/photo-1558961309-dbdf7177e419?q=80&w=600&auto=format&fit=crop", "Bột mì, sữa bột, đường cát, bơ thực vật, hương sữa tổng hợp", true, "Bánh Gấu Nhân Kem Sữa Béo", 30000m, 4.7000000000000002, 200, "200g" },
                    { 8, 4, "Lá rong biển khô tẩm gia vị muối ớt cay mặn, sấy giòn rụm rồi phi thơm cùng tỏi băm ngập dầu vàng giòn. Thích hợp cho người ăn kiêng thanh đạm.", "https://images.unsplash.com/photo-1607349913338-fca6f7fc42d0?q=80&w=600&auto=format&fit=crop", "Rong biển nhập khẩu, tỏi phi giòn, muối, ớt, đường", true, "Rong Biển Cháy Tỏi Giòn Tan", 35000m, 4.7999999999999998, 90, "90g" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_SnackId",
                table: "OrderItems",
                column: "SnackId");

            migrationBuilder.CreateIndex(
                name: "IX_Snacks_CategoryId",
                table: "Snacks",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Snacks");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
