using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estimmo.Data.Migrations
{
    public partial class Places : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW place AS
                SELECT type, id, name, unaccent(replace(lower(name), '-', ' ')) AS search_name FROM
                (
                    SELECT 1 AS type, id, name FROM region
                    UNION
                    SELECT 2 AS type, id, name FROM department
                    UNION
                    SELECT 3 AS type, id, name FROM town
                ) s
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW place");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,");
        }
    }
}
