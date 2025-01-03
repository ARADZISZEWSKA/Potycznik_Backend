using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Potycznik_Backend.Migrations
{
    /// <inheritdoc />
    public partial class changingInventoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRecords_Products_ProductId",
                table: "InventoryRecords");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "InventoryRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRecords_Products_ProductId",
                table: "InventoryRecords",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRecords_Products_ProductId",
                table: "InventoryRecords");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "InventoryRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRecords_Products_ProductId",
                table: "InventoryRecords",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
