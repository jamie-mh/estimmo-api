using Microsoft.EntityFrameworkCore.Migrations;

namespace Estimmo.Data.Migrations
{
    public partial class StreetNameSuffix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "street_number_suffix",
                table: "property_sale",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "street_number_suffix",
                table: "property_sale");
        }
    }
}
