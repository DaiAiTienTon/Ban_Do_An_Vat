using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ban_Do_An_Vat.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "7d9936c7-b65e-4bdb-851e-ca04a9fbb0e9", "AQAAAAIAAYagAAAAEAumbZkElg4iZqGQ4ufoZftOoFLjWbOJ+x4RFOVbSvzbl2gCw+LrtEYOhafOApAdeA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4bb26b2c-2f21-42c9-89e6-21a89887b624", "AQAAAAIAAYagAAAAEOuoQt3hu+V3X6AF9+q9LWvMTxgy9Nz0KiVOvrGFVjT9FViy5XOneVDVWyaal2p8Fg==" });
        }
    }
}
