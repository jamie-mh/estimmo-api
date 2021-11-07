using Microsoft.EntityFrameworkCore.Migrations;

namespace Estimmo.Data.Migrations
{
    public partial class GeometryIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "property_sale_coordinates_idx",
                table: "property_sale",
                newName: "IX_property_sale_coodinates");

            migrationBuilder.CreateIndex(
                name: "IX_town_geometry",
                table: "town",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_section_geometry",
                table: "section",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_parcel_geometry",
                table: "parcel",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_town_geometry",
                table: "town");

            migrationBuilder.DropIndex(
                name: "IX_section_geometry",
                table: "section");

            migrationBuilder.DropIndex(
                name: "IX_parcel_geometry",
                table: "parcel");

            migrationBuilder.RenameIndex(
                name: "IX_property_sale_coodinates",
                table: "property_sale",
                newName: "property_sale_coordinates_idx");
        }
    }
}
