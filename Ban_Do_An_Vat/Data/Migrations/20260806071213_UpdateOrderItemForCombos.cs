using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ban_Do_An_Vat.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderItemForCombos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Snacks_SnackId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "SnackId",
                table: "OrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ComboId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "fe59f30e-517e-43e7-b8a3-e4597a545144", "AQAAAAIAAYagAAAAEH32y0kIJNxljoHrvlgilV3gMfRV4U/Hiv50b8uz2yt0XASkC8eRLXxvWDYp5SP61A==" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ComboId",
                table: "OrderItems",
                column: "ComboId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Combos_ComboId",
                table: "OrderItems",
                column: "ComboId",
                principalTable: "Combos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Snacks_SnackId",
                table: "OrderItems",
                column: "SnackId",
                principalTable: "Snacks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Combos_ComboId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Snacks_SnackId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ComboId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ComboId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "SnackId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "7ec190f8-d834-4140-8b16-f26370bfa024", "AQAAAAIAAYagAAAAEMiRCMDubglhwY1p1ubQTREsevrhy8721YkfYZWoVl99e7w69VfaMzJeOeeRCVSyVg==" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Snacks_SnackId",
                table: "OrderItems",
                column: "SnackId",
                principalTable: "Snacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
