using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ban_Do_An_Vat.Data.MigrationsPostgres
{
    /// <inheritdoc />
    public partial class AddImageDataToPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Snacks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Snacks",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Combos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Combos",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Categories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Categories",
                type: "bytea",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "2dab26c7-0e14-438c-94e9-14581e0a3f72", "AQAAAAIAAYagAAAAED8KYW4KNhCMq9bok9REwxKsUzYP/rvZQMGwG8rIcuaeFDgjkymGPjmOEt/Gaudx6Q==" });

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
                values: new object[] { "64a0f56e-06e7-42e0-80ce-76b77c93809e", "AQAAAAIAAYagAAAAEC8zLj5kHR/QrgL0vO9cLC+7blaxynzfTJE6iK8mOx1k/EyLzc0YNxQg6nC4AaynzA==" });
        }
    }
}
