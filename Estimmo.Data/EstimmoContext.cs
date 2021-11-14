using Estimmo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Estimmo.Data
{
    public class EstimmoContext : DbContext
    {
        public EstimmoContext()
        {
        }

        public EstimmoContext(DbContextOptions<EstimmoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Town> Towns { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<PropertySale> PropertySales { get; set; }
        public virtual DbSet<RegionAverageValue> RegionAverageValues { get; set; }
        public virtual DbSet<DepartmentAverageValue> DepartmentAverageValues { get; set; }
        public virtual DbSet<TownAverageValue> TownAverageValues { get; set; }
        public virtual DbSet<SectionAverageValue> SectionAverageValues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(options =>
            {
                options.UseNetTopologySuite();
                options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<Region>(entity =>
            {
                entity.ToTable("region");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.HasIndex(e => e.Geometry).HasMethod("gist");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.HasIndex(e => e.Geometry).HasMethod("gist");

                entity.HasOne(d => d.Region)
                    .WithMany(r => r.Departments)
                    .HasForeignKey(d => d.RegionId);
            });

            modelBuilder.Entity<Town>(entity =>
            {
                entity.ToTable("town");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id").IsRequired();

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.HasIndex(e => e.Geometry).HasMethod("gist");

                entity.HasOne(t => t.Department)
                    .WithMany(d => d.Towns)
                    .HasForeignKey(t => t.DepartmentId);
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.ToTable("section");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TownId).HasColumnName("town_id");

                entity.Property(e => e.Prefix).HasColumnName("prefix").IsRequired();

                entity.Property(e => e.Code).HasColumnName("code").IsRequired();

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.HasIndex(e => e.Geometry).HasMethod("gist");

                entity.HasOne(s => s.Town)
                    .WithMany(t => t.Sections)
                    .HasForeignKey(s => s.TownId);
            });

            modelBuilder.Entity<PropertySale>(entity =>
            {
                entity.ToTable("property_sale");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Date).HasColumnName("date").HasColumnType("date").IsRequired();

                entity.Property(e => e.StreetNumber).HasColumnName("street_number");

                entity.Property(e => e.StreetNumberSuffix).HasColumnName("street_number_suffix");

                entity.Property(e => e.StreetName).HasColumnName("street_name").IsRequired();

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("smallint")
                    .HasConversion(v => (int) v, v => (PropertyType) v);

                entity.Property(e => e.PostCode).HasColumnName("post_code");

                entity.Property(e => e.BuildingSurfaceArea).HasColumnName("building_surface_area");

                entity.Property(e => e.LandSurfaceArea).HasColumnName("land_surface_area");

                entity.Property(e => e.RoomCount).HasColumnName("room_count");

                entity.Property(e => e.Value).HasColumnName("value").HasColumnType("money");

                entity.Property(e => e.SectionId).HasColumnName("section_id");

                entity.Property(e => e.Coordinates).HasColumnName("coodinates").HasColumnType("geography").IsRequired();

                entity.HasIndex(e => e.Coordinates).HasMethod("gist");

                entity.HasOne(p => p.Section)
                    .WithMany(p => p.PropertySales)
                    .HasForeignKey(p => p.SectionId);
            });

            modelBuilder.Entity<RegionAverageValue>(entity =>
            {
                entity.ToView("region_avg_value");

                entity.HasKey(e => new { e.Id, e.Type });

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<DepartmentAverageValue>(entity =>
            {
                entity.ToView("department_avg_value");

                entity.HasKey(e => new { e.Id, e.Type });

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.Region)
                    .WithMany(r => r.DepartmentAverageValues)
                    .HasForeignKey(d => d.RegionId);
            });

            modelBuilder.Entity<TownAverageValue>(entity =>
            {
                entity.ToView("town_avg_value");

                entity.HasKey(e => new { e.Id, e.Type });

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(t => t.Department)
                    .WithMany(d => d.TownAverageValues)
                    .HasForeignKey(t => t.DepartmentId);
            });

            modelBuilder.Entity<SectionAverageValue>(entity =>
            {
                entity.ToView("section_avg_value");

                entity.HasKey(e => new { e.Id, e.Type });

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.TownId).HasColumnName("town_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(s => s.Town)
                    .WithMany(t => t.SectionAverageValues)
                    .HasForeignKey(s => s.TownId);
            });
        }
    }
}
