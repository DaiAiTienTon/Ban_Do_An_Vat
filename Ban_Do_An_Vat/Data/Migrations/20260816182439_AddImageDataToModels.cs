using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ban_Do_An_Vat.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageDataToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Snacks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Snacks",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Combos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Combos",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Categories",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b0e3c516-d0dd-4394-bbbd-c74e17b83df6", "AQAAAAIAAYagAAAAEGZBBP+17dIn6Z96VkslZ5aW/IgJ/ILLoY1LqMUMifANtbE5+Q08UgqjkwiABm3Iew==" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Snacks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Snacks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Snacks",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Snacks",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Snacks",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Snacks",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Snacks",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Snacks",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ImageContentType", "ImageData" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Snacks");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Snacks");

            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "fe59f30e-517e-43e7-b8a3-e4597a545144", "AQAAAAIAAYagAAAAEH32y0kIJNxljoHrvlgilV3gMfRV4U/Hiv50b8uz2yt0XASkC8eRLXxvWDYp5SP61A==" });
        }
    }
}
