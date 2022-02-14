using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estimmo.Data.Migrations
{
    public partial class PlacesPostcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW place");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW place AS
                SELECT type, id, name, unaccent(replace(lower(name), '-', ' ')) AS search_name, post_code, geometry FROM
                (
                    SELECT 1 AS type, id, name, NULL as post_code, geometry FROM region
                    UNION
                    SELECT 2 AS type, id, name, NULL as post_code, geometry FROM department
                    UNION
                    SELECT 3 AS type, id, name, post_code, geometry FROM town
                ) s
            ");

            migrationBuilder.Sql("CREATE INDEX ix_place_geometry ON place USING GIST(geometry)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW place");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW place AS
                SELECT type, id, name, unaccent(replace(lower(name), '-', ' ')) AS search_name, geometry FROM
                (
                    SELECT 1 AS type, id, name, geometry FROM region
                    UNION
                    SELECT 2 AS type, id, name, geometry FROM department
                    UNION
                    SELECT 3 AS type, id, name, geometry FROM town
                ) s
            ");
        }
    }
}
