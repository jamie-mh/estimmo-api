using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estimmo.Data.Migrations
{
    public partial class FranceAverages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW france_avg_value AS
                SELECT ps.type, AVG(ps.value / CAST(ps.building_surface_area AS money)) AS value FROM property_sale ps
                GROUP BY ps.type
            ");

            migrationBuilder.Sql("CREATE UNIQUE INDEX PK_france_avg_value ON france_avg_value (type)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE MATERIALIZED VIEW france_avg_value");
        }
    }
}
