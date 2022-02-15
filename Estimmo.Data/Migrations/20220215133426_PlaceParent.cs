using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estimmo.Data.Migrations
{
    public partial class PlaceParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW place");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW place AS
                SELECT type, id, name, unaccent(replace(lower(name), '-', ' ')) AS search_name, post_code, parent_type, parent_id, geometry FROM
                (
                    SELECT 1 AS type, id, name, NULL AS post_code, NULL AS parent_type, NULL AS parent_id, geometry FROM region
                    UNION
                    SELECT 2 AS type, d.id, d.name, NULL AS post_code, 1 AS parent_type, r.id AS parent_id, d.geometry FROM department d
                    INNER JOIN region r on d.region_id = r.id
                    UNION
                    SELECT 3 AS type, t.id, t.name, t.post_code, 2 AS parent_type, d.id AS parent_id, t.geometry FROM town t
                    INNER JOIN department d on t.department_id = d.id
                ) s
            ");

            migrationBuilder.Sql("CREATE INDEX ix_place_geometry ON place USING GIST(geometry)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
