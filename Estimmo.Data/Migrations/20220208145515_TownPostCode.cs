using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estimmo.Data.Migrations
{
    public partial class TownPostCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "post_code",
                table: "town",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "post_code",
                table: "town");
        }
    }
}
