using Microsoft.EntityFrameworkCore.Migrations;

namespace Estimmo.Data.Migrations
{
    public partial class CoordinatesIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "property_sale_coordinates_idx",
                table: "property_sale",
                column: "coodinates")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "property_sale_coordinates_idx",
                table: "property_sale");
        }
    }
}
