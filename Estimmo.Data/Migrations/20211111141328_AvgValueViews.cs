using Microsoft.EntityFrameworkCore.Migrations;

namespace Estimmo.Data.Migrations
{
    public partial class AvgValueViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW region_avg_value AS
                SELECT r.id, ps.type, AVG(ps.value / CAST(ps.building_surface_area AS money)) AS value FROM property_sale ps
                LEFT JOIN parcel p on ps.parcel_id = p.id
                LEFT JOIN section s on p.section_id = s.id
                LEFT JOIN town t on p.town_id = t.id
                LEFT JOIN department d on t.department_id = d.id
                LEFT JOIN region r on d.region_id = r.id
                GROUP BY r.id, ps.type
            ");

            migrationBuilder.Sql("CREATE UNIQUE INDEX PK_region_avg_value ON region_avg_value (id, type)");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW department_avg_value AS
                SELECT d.id, ps.type, d.region_id, AVG(ps.value / CAST(ps.building_surface_area AS money)) AS value FROM property_sale ps
                LEFT JOIN parcel p on ps.parcel_id = p.id
                LEFT JOIN section s on p.section_id = s.id
                LEFT JOIN town t on p.town_id = t.id
                LEFT JOIN department d on t.department_id = d.id
                GROUP BY d.id, ps.type
            ");

            migrationBuilder.Sql("CREATE UNIQUE INDEX PK_department_avg_value ON department_avg_value (id, type)");
            migrationBuilder.Sql("CREATE INDEX IX_department_avg_value_region_id ON department_avg_value (region_id)");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW town_avg_value AS
                SELECT t.id, ps.type, t.department_id, AVG(ps.value / CAST(ps.building_surface_area AS money)) AS value FROM property_sale ps
                LEFT JOIN parcel p on ps.parcel_id = p.id
                LEFT JOIN section s on p.section_id = s.id
                LEFT JOIN town t on p.town_id = t.id
                GROUP BY t.id, ps.type
            ");

            migrationBuilder.Sql("CREATE UNIQUE INDEX PK_town_avg_value ON town_avg_value (id, type)");
            migrationBuilder.Sql("CREATE INDEX IX_town_avg_value_department_id ON town_avg_value (department_id)");

            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW section_avg_value AS
                SELECT s.id, ps.type, s.town_id, AVG(ps.value / CAST(ps.building_surface_area AS money)) AS value FROM property_sale ps
                LEFT JOIN parcel p on ps.parcel_id = p.id
                LEFT JOIN section s on p.section_id = s.id
                GROUP BY s.id, ps.type
            ");

            migrationBuilder.Sql("CREATE UNIQUE INDEX PK_section_avg_value ON section_avg_value (id, type)");
            migrationBuilder.Sql("CREATE INDEX IX_section_avg_value_town_id ON section_avg_value (town_id)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW region_avg_value");
            migrationBuilder.Sql("DROP MATERIALIZED VIEW department_avg_value");
            migrationBuilder.Sql("DROP MATERIALIZED VIEW town_avg_value");
            migrationBuilder.Sql("DROP MATERIALIZED VIEW section_avg_value");
        }
    }
}
