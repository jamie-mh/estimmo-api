using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW france_avg_value AS
                SELECT 0 AS type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                UNION
                SELECT ps.type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                GROUP BY ps.type;

                CREATE UNIQUE INDEX pk_france_avg_value ON france_avg_value(type);

                CREATE MATERIALIZED VIEW france_avg_value_by_year AS
                SELECT 0 AS type, CAST(EXTRACT(YEAR FROM ps.date) AS smallint) AS year, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                GROUP BY year
                UNION
                SELECT ps.type, CAST(EXTRACT(YEAR FROM ps.date) AS smallint) AS year, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                GROUP BY ps.type, year;

                CREATE UNIQUE INDEX pk_france_avg_value_by_year ON france_avg_value_by_year(type, year);
            ");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW region_avg_value AS
                SELECT r.id, 0 AS type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                INNER JOIN department d on t.department_id = d.id
                INNER JOIN region r on d.region_id = r.id
                GROUP BY r.id
                UNION
                SELECT r.id, ps.type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                INNER JOIN department d on t.department_id = d.id
                INNER JOIN region r on d.region_id = r.id
                GROUP BY r.id, ps.type;

                CREATE UNIQUE INDEX pk_region_avg_value ON region_avg_value(id, type);

                CREATE MATERIALIZED VIEW region_avg_value_by_year AS
                SELECT r.id, 0 AS type, CAST(EXTRACT(YEAR FROM ps.date) AS smallint) AS year, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                INNER JOIN department d on t.department_id = d.id
                INNER JOIN region r on d.region_id = r.id
                GROUP BY r.id, year
                UNION
                SELECT r.id, ps.type, CAST(EXTRACT(YEAR FROM ps.date) AS smallint) AS year, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                INNER JOIN department d on t.department_id = d.id
                INNER JOIN region r on d.region_id = r.id
                GROUP BY r.id, ps.type, year;

                CREATE UNIQUE INDEX pk_region_avg_value_by_year ON region_avg_value_by_year(id, type, year);
            ");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW department_avg_value AS
                SELECT d.id, d.region_id, 0 AS type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                INNER JOIN department d on t.department_id = d.id
                GROUP BY d.id
                UNION
                SELECT d.id, d.region_id, ps.type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                INNER JOIN department d on t.department_id = d.id
                GROUP BY d.id, ps.type;

                CREATE UNIQUE INDEX pk_department_avg_value ON department_avg_value(id, type);
                CREATE INDEX ix_department_avg_value_region_id ON department_avg_value(region_id);

                CREATE MATERIALIZED VIEW department_avg_value_by_year AS
                SELECT d.id, d.region_id, 0 AS type, CAST(EXTRACT(YEAR FROM ps.date) AS smallint) AS year, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                INNER JOIN department d on t.department_id = d.id
                GROUP BY d.id, year
                UNION
                SELECT d.id, d.region_id, ps.type, CAST(EXTRACT(YEAR FROM ps.date) AS smallint) AS year, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                INNER JOIN department d on t.department_id = d.id
                GROUP BY d.id, ps.type, year;

                CREATE UNIQUE INDEX pk_department_avg_value_by_year ON department_avg_value_by_year(id, type, year);
                CREATE INDEX ix_department_avg_value_by_year_region_id ON department_avg_value_by_year(region_id);
            ");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW town_avg_value AS
                SELECT t.id, t.department_id, 0 AS type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                GROUP BY t.id
                UNION
                SELECT t.id, t.department_id, ps.type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                GROUP BY t.id, ps.type;

                CREATE UNIQUE INDEX pk_town_avg_value ON town_avg_value(id, type);
                CREATE INDEX ix_town_avg_value ON town_avg_value(department_id);

                CREATE MATERIALIZED VIEW town_avg_value_by_year AS
                SELECT t.id, t.department_id, 0 AS type, CAST(EXTRACT(YEAR FROM ps.date) AS smallint) AS year, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                GROUP BY t.id, year
                UNION
                SELECT t.id, t.department_id, ps.type, CAST(EXTRACT(YEAR FROM ps.date) AS smallint) AS year, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                INNER JOIN town t on s.town_id = t.id
                GROUP BY t.id, ps.type, year;

                CREATE UNIQUE INDEX pk_town_avg_value_by_year ON town_avg_value_by_year(id, type, year);
                CREATE INDEX ix_town_avg_value_by_year ON town_avg_value_by_year(department_id);
            ");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW section_avg_value AS
                SELECT s.id, s.town_id, 0 AS type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                GROUP BY s.id
                UNION
                SELECT s.id, s.town_id, ps.type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                GROUP BY s.id, ps.type;

                CREATE UNIQUE INDEX pk_section_avg_value ON section_avg_value(id, type);
                CREATE INDEX ix_section_avg_value_town_id ON section_avg_value(town_id);

                CREATE MATERIALIZED VIEW section_avg_value_by_year AS
                SELECT s.id, s.town_id, 0 AS type, CAST(EXTRACT(YEAR FROM ps.date) AS smallint) AS year, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                GROUP BY s.id, year
                UNION
                SELECT s.id, s.town_id, ps.type, CAST(EXTRACT(YEAR FROM ps.date) AS smallint) AS year, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value FROM property_sale ps
                INNER JOIN section s on ps.section_id = s.id
                GROUP BY s.id, ps.type, year;

                CREATE UNIQUE INDEX pk_section_avg_value_by_year ON section_avg_value_by_year(id, type, year);
                CREATE INDEX ix_section_avg_value_by_year_town_id ON section_avg_value_by_year(town_id);
            ");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW place AS
                SELECT type, id, name, UNACCENT(REPLACE(LOWER(name), '-', ' ')) AS search_name, post_code, parent_type, parent_id, geometry FROM
                (
                    SELECT 1 AS type, id, name, NULL AS post_code, NULL AS parent_type, NULL AS parent_id, geometry FROM region
                    UNION
                    SELECT 2 AS type, d.id, d.name, NULL AS post_code, 1 AS parent_type, r.id AS parent_id, d.geometry FROM department d
                    INNER JOIN region r on d.region_id = r.id
                    UNION
                    SELECT 3 AS type, t.id, t.name, t.post_code, 2 AS parent_type, d.id AS parent_id, t.geometry FROM town t
                    INNER JOIN department d on t.department_id = d.id
                    UNION
                    SELECT 4 AS type, a.id, CONCAT(a.number, a.suffix, ' ', s.name, ' ', t.name) AS name, a.post_code, 3 AS parent_type, s.town_id AS parent_id, a.coordinates AS geometry FROM address a
                    INNER JOIN street s on a.street_id = s.id
                    INNER JOIN town t on s.town_id = t.id
                ) s;

                CREATE UNIQUE INDEX pk_place ON place(type, id);
                CREATE INDEX ix_place_search_name ON place USING gin (search_name gin_trgm_ops);
                CREATE INDEX ix_place_post_code ON place(post_code);
                CREATE INDEX ix_place_geometry ON place USING GIST(geometry);
            ");
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
    }
}
