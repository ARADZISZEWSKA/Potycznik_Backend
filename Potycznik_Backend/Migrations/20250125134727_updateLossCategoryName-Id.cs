using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Potycznik_Backend.Migrations
{
    /// <inheritdoc />
    public partial class updateLossCategoryNameId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Losses");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Losses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Losses");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Losses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
