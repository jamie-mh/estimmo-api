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
                name: "property_sale",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    street_number = table.Column<short>(type: "smallint", nullable: true),
                    street_name = table.Column<string>(type: "text", nullable: true),
                    post_code = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "smallint", nullable: false),
                    building_surface_area = table.Column<int>(type: "integer", nullable: false),
                    land_surface_area = table.Column<int>(type: "integer", nullable: false),
                    room_count = table.Column<short>(type: "smallint", nullable: false),
                    value = table.Column<decimal>(type: "money", nullable: false),
                    coodinates = table.Column<Point>(type: "geography", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_sale", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "property_sale");
        }
    }
}
