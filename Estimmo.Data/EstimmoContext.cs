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
        public virtual DbSet<FranceValueStats> FranceValueStats { get; set; }
        public virtual DbSet<FranceValueStatsByYear> FranceValueStatsByYear { get; set; }
        public virtual DbSet<RegionValueStats> RegionValueStats { get; set; }
        public virtual DbSet<RegionValueStatsByYear> RegionValueStatsByYear { get; set; }
        public virtual DbSet<DepartmentValueStats> DepartmentValueStats { get; set; }
        public virtual DbSet<DepartmentValueStatsByYear> DepartmentValueStatsByYear { get; set; }
        public virtual DbSet<TownValueStats> TownValueStats { get; set; }
        public virtual DbSet<TownValueStatsByYear> TownValueStatsByYear { get; set; }
        public virtual DbSet<SectionValueStats> SectionValueStats { get; set; }
        public virtual DbSet<SectionValueStatsByYear> SectionValueStatsByYear { get; set; }
        public virtual DbSet<Place> Places { get; set; }
        public virtual DbSet<Street> Streets { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<SaidPlace> SaidPlaces { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(options =>
            {
                options.UseNetTopologySuite();
                options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            optionsBuilder.UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.HasPostgresExtension("unaccent");
            modelBuilder.HasPostgresExtension("pg_trgm");
            modelBuilder.HasPostgresExtension("tdigest");

            modelBuilder.Entity<Region>(entity =>
            {
                entity.ToTable("region");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.HasIndex(e => e.Geometry).HasMethod("gist");

                entity.HasMany(e => e.ValueStats)
                    .WithOne(e => e.Region)
                    .HasForeignKey(e => e.Id);

                entity.HasMany(e => e.ValueStatsByYear)
                    .WithOne(e => e.Region)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RegionId).HasColumnName("region_id").IsRequired();

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.HasIndex(e => e.Geometry).HasMethod("gist");

                entity.HasOne(d => d.Region)
                    .WithMany(r => r.Departments)
                    .HasForeignKey(d => d.RegionId);

                entity.HasMany(e => e.ValueStats)
                    .WithOne(e => e.Department)
                    .HasForeignKey(e => e.Id);

                entity.HasMany(e => e.ValueStatsByYear)
                    .WithOne(e => e.Department)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<Town>(entity =>
            {
                entity.ToTable("town");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id").IsRequired();

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.Property(e => e.PostCode).HasColumnName("post_code");

                entity.HasIndex(e => e.Geometry).HasMethod("gist");

                entity.HasOne(t => t.Department)
                    .WithMany(d => d.Towns)
                    .HasForeignKey(t => t.DepartmentId);

                entity.HasMany(e => e.ValueStats)
                    .WithOne(e => e.Town)
                    .HasForeignKey(e => e.Id);

                entity.HasMany(e => e.ValueStatsByYear)
                    .WithOne(e => e.Town)
                    .HasForeignKey(e => e.Id);

                entity.HasMany(e => e.Streets)
                    .WithOne(e => e.Town)
                    .HasForeignKey(e => e.TownId);
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.ToTable("section");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TownId).HasColumnName("town_id").IsRequired();

                entity.Property(e => e.Prefix).HasColumnName("prefix").IsRequired();

                entity.Property(e => e.Code).HasColumnName("code").IsRequired();

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.HasIndex(e => e.Geometry).HasMethod("gist");

                entity.HasOne(s => s.Town)
                    .WithMany(t => t.Sections)
                    .HasForeignKey(s => s.TownId);

                entity.HasMany(e => e.ValueStats)
                    .WithOne(e => e.Section)
                    .HasForeignKey(e => e.Id);

                entity.HasMany(e => e.ValueStatsByYear)
                    .WithOne(e => e.Section)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<PropertySale>(entity =>
            {
                entity.ToTable("property_sale");

                entity.HasKey(e => e.Hash);

                entity.Property(e => e.Hash).HasColumnName("hash");

                entity.Property(e => e.Date).HasColumnName("date").HasColumnType("date").IsRequired();

                entity.Property(e => e.StreetNumber).HasColumnName("street_number");

                entity.Property(e => e.StreetNumberSuffix).HasColumnName("street_number_suffix");

                entity.Property(e => e.StreetName).HasColumnName("street_name").IsRequired();

                entity.Property(e => e.BuildingSurfaceArea).HasColumnName("building_surface_area");

                entity.Property(e => e.LandSurfaceArea).HasColumnName("land_surface_area");

                entity.Property(e => e.RoomCount).HasColumnName("room_count");

                entity.Property(e => e.Value).HasColumnName("value").HasColumnType("money");

                entity.Property(e => e.SectionId).HasColumnName("section_id").IsRequired();

                entity.Property(e => e.Coordinates).HasColumnName("coordinates").HasColumnType("geography").IsRequired();

                entity.HasIndex(e => e.Coordinates).HasMethod("gist");

                entity.HasOne(p => p.Section)
                    .WithMany(p => p.PropertySales)
                    .HasForeignKey(p => p.SectionId);
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.Suffix).HasColumnName("suffix");

                entity.Property(e => e.PostCode).HasColumnName("post_code").IsRequired();

                entity.Property(e => e.StreetId).HasColumnName("street_id").IsRequired();

                entity.Property(e => e.Coordinates).HasColumnName("coordinates").IsRequired();

                entity.HasIndex(e => e.Coordinates).HasMethod("gist");
            });

            modelBuilder.Entity<Street>(entity =>
            {
                entity.ToTable("street");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.TownId).HasColumnName("town_id").IsRequired();

                entity.Property(e => e.Coordinates).HasColumnName("coordinates");

                entity.HasMany(e => e.Addresses)
                    .WithOne(e => e.Street)
                    .HasForeignKey(e => e.StreetId);
            });

            modelBuilder.Entity<SaidPlace>(entity =>
            {
                entity.ToTable("said_place");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.PostCode).HasColumnName("post_code").IsRequired();

                entity.Property(e => e.TownId).HasColumnName("town_id").IsRequired();

                entity.Property(e => e.Coordinates).HasColumnName("coordinates").IsRequired();

                entity.HasIndex(e => e.Coordinates).HasMethod("gist");
            });

            modelBuilder.Entity<FranceValueStats>(entity =>
            {
                entity.HasKey(e => e.Type);

                entity.ToView("france_value_stats");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Median).HasColumnName("median");

                entity.Property(e => e.StandardDeviation).HasColumnName("standard_deviation");
            });

            modelBuilder.Entity<FranceValueStatsByYear>(entity =>
            {
                entity.HasKey(e => new { e.Type, e.Year });

                entity.ToView("france_value_stats_by_year");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Median).HasColumnName("median");

                entity.Property(e => e.StandardDeviation).HasColumnName("standard_deviation");
            });

            modelBuilder.Entity<RegionValueStats>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type });

                entity.ToView("region_value_stats");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Median).HasColumnName("median");

                entity.Property(e => e.StandardDeviation).HasColumnName("standard_deviation");
            });

            modelBuilder.Entity<RegionValueStatsByYear>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.Year });

                entity.ToView("region_value_stats_by_year");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Median).HasColumnName("median");

                entity.Property(e => e.StandardDeviation).HasColumnName("standard_deviation");
            });

            modelBuilder.Entity<DepartmentValueStats>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type });

                entity.ToView("department_value_stats");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Median).HasColumnName("median");

                entity.Property(e => e.StandardDeviation).HasColumnName("standard_deviation");

                entity.HasOne(d => d.Region)
                    .WithMany(r => r.DepartmentValueStats)
                    .HasForeignKey(d => d.RegionId);
            });

            modelBuilder.Entity<DepartmentValueStatsByYear>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.Year });

                entity.ToView("department_value_stats_by_year");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Median).HasColumnName("median");

                entity.Property(e => e.StandardDeviation).HasColumnName("standard_deviation");

                entity.HasOne(d => d.Region)
                    .WithMany(r => r.DepartmentValueStatsByYear)
                    .HasForeignKey(d => d.RegionId);
            });

            modelBuilder.Entity<TownValueStats>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type });

                entity.ToView("town_value_stats");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Median).HasColumnName("median");

                entity.Property(e => e.StandardDeviation).HasColumnName("standard_deviation");

                entity.HasOne(t => t.Department)
                    .WithMany(d => d.TownValueStats)
                    .HasForeignKey(t => t.DepartmentId);
            });

            modelBuilder.Entity<TownValueStatsByYear>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.Year });

                entity.ToView("town_value_stats_by_year");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Median).HasColumnName("median");

                entity.Property(e => e.StandardDeviation).HasColumnName("standard_deviation");

                entity.HasOne(t => t.Department)
                    .WithMany(d => d.TownValueStatsByYear)
                    .HasForeignKey(t => t.DepartmentId);
            });

            modelBuilder.Entity<SectionValueStats>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type });

                entity.ToView("section_value_stats");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.TownId).HasColumnName("town_id");

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Median).HasColumnName("median");

                entity.Property(e => e.StandardDeviation).HasColumnName("standard_deviation");

                entity.HasOne(s => s.Town)
                    .WithMany(t => t.SectionValueStats)
                    .HasForeignKey(s => s.TownId);
            });

            modelBuilder.Entity<SectionValueStatsByYear>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.Year });

                entity.ToView("section_value_stats_by_year");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.TownId).HasColumnName("town_id");

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Median).HasColumnName("median");

                entity.Property(e => e.StandardDeviation).HasColumnName("standard_deviation");

                entity.HasOne(s => s.Town)
                    .WithMany(t => t.SectionValueStatsByYear)
                    .HasForeignKey(s => s.TownId);
            });

            modelBuilder.Entity<Place>(entity =>
            {
                entity.HasKey(e => new { e.Type, e.Id });

                entity.ToView("place");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.ShortName).HasColumnName("short_name");

                entity.Property(e => e.SearchName).HasColumnName("search_name");

                entity.Property(e => e.PostCode).HasColumnName("post_code");

                entity.Property(e => e.ParentType).HasColumnName("parent_type");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.IsSearchable).HasColumnName("is_searchable");

                entity.Property(e => e.Geometry).HasColumnName("geometry");

                entity.HasOne(p => p.Parent)
                    .WithMany()
                    .HasForeignKey(p => new { p.ParentType, p.ParentId });
            });
        }
    }
}
