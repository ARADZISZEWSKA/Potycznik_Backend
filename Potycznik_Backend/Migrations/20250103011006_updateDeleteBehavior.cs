using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Potycznik_Backend.Migrations
{
    /// <inheritdoc />
    public partial class updateDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRecords_Products_ProductId",
                table: "InventoryRecords");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "InventoryRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRecords_Products_ProductId",
                table: "InventoryRecords",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRecords_Products_ProductId",
                table: "InventoryRecords");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "InventoryRecords",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRecords_Products_ProductId",
                table: "InventoryRecords",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
