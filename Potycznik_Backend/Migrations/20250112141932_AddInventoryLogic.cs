using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Potycznik_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRecords_Inventories_InventoryId",
                table: "InventoryRecords");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Date",
                table: "Inventories",
                column: "Date",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRecords_Inventories_InventoryId",
                table: "InventoryRecords",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRecords_Inventories_InventoryId",
                table: "InventoryRecords");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Date",
                table: "Inventories");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRecords_Inventories_InventoryId",
                table: "InventoryRecords",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id");
        }
    }
}
