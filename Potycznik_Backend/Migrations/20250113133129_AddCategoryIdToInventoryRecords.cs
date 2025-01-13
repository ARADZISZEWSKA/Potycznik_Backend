using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Potycznik_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIdToInventoryRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "InventoryRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "InventoryRecords");
        }
    }
}
