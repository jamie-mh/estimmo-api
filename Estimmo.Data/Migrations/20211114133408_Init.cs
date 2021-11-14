using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Estimmo.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "region",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    geometry = table.Column<Geometry>(type: "geography", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_region", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    region_id = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    geometry = table.Column<Geometry>(type: "geography", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => x.id);
                    table.ForeignKey(
                        name: "FK_department_region_region_id",
                        column: x => x.region_id,
                        principalTable: "region",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "town",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    department_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    geometry = table.Column<Geometry>(type: "geography", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_town", x => x.id);
                    table.ForeignKey(
                        name: "FK_town_department_department_id",
                        column: x => x.department_id,
                        principalTable: "department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "section",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    town_id = table.Column<string>(type: "text", nullable: true),
                    prefix = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
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
                name: "property_sale",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    street_number = table.Column<short>(type: "smallint", nullable: true),
                    street_number_suffix = table.Column<string>(type: "text", nullable: true),
                    street_name = table.Column<string>(type: "text", nullable: false),
                    post_code = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "smallint", nullable: false),
                    building_surface_area = table.Column<int>(type: "integer", nullable: false),
                    land_surface_area = table.Column<int>(type: "integer", nullable: false),
                    room_count = table.Column<short>(type: "smallint", nullable: false),
                    value = table.Column<decimal>(type: "money", nullable: false),
                    section_id = table.Column<string>(type: "text", nullable: true),
                    coodinates = table.Column<Point>(type: "geography", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_sale", x => x.id);
                    table.ForeignKey(
                        name: "FK_property_sale_section_section_id",
                        column: x => x.section_id,
                        principalTable: "section",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_department_geometry",
                table: "department",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_department_region_id",
                table: "department",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "IX_property_sale_coodinates",
                table: "property_sale",
                column: "coodinates")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_property_sale_section_id",
                table: "property_sale",
                column: "section_id");

            migrationBuilder.CreateIndex(
                name: "IX_region_geometry",
                table: "region",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_section_geometry",
                table: "section",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_section_town_id",
                table: "section",
                column: "town_id");

            migrationBuilder.CreateIndex(
                name: "IX_town_department_id",
                table: "town",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_town_geometry",
                table: "town",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "property_sale");

            migrationBuilder.DropTable(
                name: "section");

            migrationBuilder.DropTable(
                name: "town");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "region");
        }
    }
}
