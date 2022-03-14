﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.IO;
using System.Reflection;

#nullable disable

namespace Estimmo.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    subject = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    sent_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message", x => x.id);
                });

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
                    table.PrimaryKey("pk_region", x => x.id);
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
                    table.PrimaryKey("pk_department", x => x.id);
                    table.ForeignKey(
                        name: "fk_department_region_region_id",
                        column: x => x.region_id,
                        principalTable: "region",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "town",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    department_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    post_code = table.Column<string>(type: "text", nullable: true),
                    geometry = table.Column<Geometry>(type: "geography", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_town", x => x.id);
                    table.ForeignKey(
                        name: "fk_town_department_department_id",
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
                    table.PrimaryKey("pk_section", x => x.id);
                    table.ForeignKey(
                        name: "fk_section_town_town_id",
                        column: x => x.town_id,
                        principalTable: "town",
                        principalColumn: "id");
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
                    coordinates = table.Column<Point>(type: "geography", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_property_sale", x => x.id);
                    table.ForeignKey(
                        name: "fk_property_sale_section_section_id",
                        column: x => x.section_id,
                        principalTable: "section",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "street",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    town_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_street", x => x.id);
                    table.ForeignKey(
                        name: "fk_street_town_town_id",
                        column: x => x.town_id,
                        principalTable: "town",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: true),
                    suffix = table.Column<string>(type: "text", nullable: true),
                    post_code = table.Column<string>(type: "text", nullable: true),
                    street_id = table.Column<string>(type: "text", nullable: false),
                    coordinates = table.Column<Geometry>(type: "geometry", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_address", x => x.id);
                    table.ForeignKey(
                        name: "fk_address_street_street_id",
                        column: x => x.street_id,
                        principalTable: "street",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_department_geometry",
                table: "department",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_department_region_id",
                table: "department",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_property_sale_coordinates",
                table: "property_sale",
                column: "coordinates")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_property_sale_section_id",
                table: "property_sale",
                column: "section_id");

            migrationBuilder.CreateIndex(
                name: "ix_region_geometry",
                table: "region",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_section_geometry",
                table: "section",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_section_town_id",
                table: "section",
                column: "town_id");

            migrationBuilder.CreateIndex(
                name: "ix_town_department_id",
                table: "town",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_town_geometry",
                table: "town",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_address_coordinates",
                table: "address",
                column: "coordinates")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_address_street_id",
                table: "address",
                column: "street_id");

            migrationBuilder.CreateIndex(
                name: "ix_street_town_id",
                table: "street",
                column: "town_id");

            ExecuteSqlFile(migrationBuilder, "france_avg_value");

            ExecuteSqlFile(migrationBuilder, "region_avg_value");

            ExecuteSqlFile(migrationBuilder, "department_avg_value");

            ExecuteSqlFile(migrationBuilder, "town_avg_value");

            ExecuteSqlFile(migrationBuilder, "section_avg_value");

            ExecuteSqlFile(migrationBuilder, "place");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP MATERIALIZED VIEW place;
                DROP MATERIALIZED VIEW france_avg_value;
                DROP MATERIALIZED VIEW france_avg_value_by_year;
                DROP MATERIALIZED VIEW region_avg_value;
                DROP MATERIALIZED VIEW region_avg_value_by_year;
                DROP MATERIALIZED VIEW department_avg_value;
                DROP MATERIALIZED VIEW department_avg_value_by_year;
                DROP MATERIALIZED VIEW town_avg_value;
                DROP MATERIALIZED VIEW town_avg_value_by_year;
                DROP MATERIALIZED VIEW section_avg_value;
                DROP MATERIALIZED VIEW section_avg_value_by_year;
            ");

            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "street");

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

        private static void ExecuteSqlFile(MigrationBuilder migrationBuilder, string name)
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Join(currentDir, "sql", $"{name}.sql");
            var sql = File.ReadAllText(path);
            migrationBuilder.Sql(sql);
        }
    }
}
