using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace Estimmo.Data.Migrations
{
    public partial class SectionsAndParcels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "section",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    town_id = table.Column<string>(type: "text", nullable: true),
                    prefix = table.Column<string>(type: "text", nullable: true),
                    code = table.Column<string>(type: "text", nullable: true),
                    geometry = table.Column<Geometry>(type: "geography", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_section", x => x.id);
                    table.ForeignKey(
                        name: "FK_section_town_town_id",
                        column: x => x.town_id,
                        principalTable: "town",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "parcel",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    town_id = table.Column<string>(type: "text", nullable: true),
                    prefix = table.Column<string>(type: "text", nullable: true),
                    section_code = table.Column<string>(type: "text", nullable: true),
                    number = table.Column<int>(type: "integer", nullable: false),
                    geometry = table.Column<Geometry>(type: "geography", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parcel", x => x.id);
                    table.ForeignKey(
                        name: "FK_parcel_section_id",
                        column: x => x.id,
                        principalTable: "section",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parcel_town_town_id",
                        column: x => x.town_id,
                        principalTable: "town",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_parcel_town_id",
                table: "parcel",
                column: "town_id");

            migrationBuilder.CreateIndex(
                name: "IX_section_town_id",
                table: "section",
                column: "town_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parcel");

            migrationBuilder.DropTable(
                name: "section");
        }
    }
}
