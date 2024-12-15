using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Potycznik_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Sample_Products : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Barcode", "CategoryId", "ExpiryDate", "Name", "Quantity", "Unit" },
                values: new object[,]
                {
                    { 1, "1234567890", 9, null, "Piwo", 100m, "Litr" },
                    { 2, "9876543210", 7, null, "Wódka", 50m, "Litr" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
