using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ban_Do_An_Vat.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponFieldsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "926bcc82-08da-4a8f-87bf-24fec8a6ac4b", "AQAAAAIAAYagAAAAELyPR3wPnlCGu5XKurVes7Bs50juqz5DgemspA25+5Krlw450iA1jG0dPoPLBI+R8A==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "dae3254e-da1d-4dce-b910-8f8bc4a90505", "AQAAAAIAAYagAAAAEF3M6/47E7DRuRKL+dpn1cDdVTiM0tMO1bRSyzBhlQ8GfhQxpzzL2R2JzDw7BA0ivg==" });
        }
    }
}
