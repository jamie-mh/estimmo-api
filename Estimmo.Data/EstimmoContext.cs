using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Estimmo.Data
{
    public class EstimmoContext : IdentityDbContext<AdminUser, AdminRole, int>
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
        public virtual DbSet<FranceAverageValue> FranceAverageValues { get; set; }
        public virtual DbSet<FranceAverageValueByYear> FranceAverageValuesByYear { get; set; }
        public virtual DbSet<RegionAverageValue> RegionAverageValues { get; set; }
        public virtual DbSet<RegionAverageValueByYear> RegionAverageValuesByYear { get; set; }
        public virtual DbSet<DepartmentAverageValue> DepartmentAverageValues { get; set; }
        public virtual DbSet<DepartmentAverageValueByYear> DepartmentAverageValuesByYear { get; set; }
        public virtual DbSet<TownAverageValue> TownAverageValues { get; set; }
        public virtual DbSet<TownAverageValueByYear> TownAverageValuesByYear { get; set; }
        public virtual DbSet<SectionAverageValue> SectionAverageValues { get; set; }
        public virtual DbSet<SectionAverageValue> SectionAverageValuesByYear { get; set; }
        public virtual DbSet<Place> Places { get; set; }
        public virtual DbSet<Message> Messages { get; set; }

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

            modelBuilder.Entity<Region>(entity =>
            {
                entity.ToTable("region");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.HasIndex(e => e.Geometry).HasMethod("gist");

                entity.HasMany(e => e.AverageValues)
                    .WithOne(e => e.Region)
                    .HasForeignKey(e => e.Id);

                entity.HasMany(e => e.AverageValuesByYear)
                    .WithOne(e => e.Region)
                    .HasForeignKey(e => e.Id);
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

                entity.HasMany(e => e.AverageValues)
                    .WithOne(e => e.Department)
                    .HasForeignKey(e => e.Id);

                entity.HasMany(e => e.AverageValuesByYear)
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

                entity.HasMany(e => e.AverageValues)
                    .WithOne(e => e.Town)
                    .HasForeignKey(e => e.Id);

                entity.HasMany(e => e.AverageValuesByYear)
                    .WithOne(e => e.Town)
                    .HasForeignKey(e => e.Id);
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

                entity.HasMany(e => e.AverageValues)
                    .WithOne(e => e.Section)
                    .HasForeignKey(e => e.Id);

                entity.HasMany(e => e.AverageValuesByYear)
                    .WithOne(e => e.Section)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<PropertySale>(entity =>
            {
                entity.ToTable("property_sale");

                entity.HasKey(e => e.Id);

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

            modelBuilder.Entity<FranceAverageValue>(entity =>
            {
                entity.HasKey(e => e.Type);

                entity.ToView("france_avg_value");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<FranceAverageValueByYear>(entity =>
            {
                entity.HasKey(e => new { e.Type, e.Year });

                entity.ToView("france_avg_value_by_year");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<RegionAverageValue>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type });

                entity.ToView("region_avg_value");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<RegionAverageValueByYear>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.Year });

                entity.ToView("region_avg_value_by_year");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<DepartmentAverageValue>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type });

                entity.ToView("department_avg_value");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.Region)
                    .WithMany(r => r.DepartmentAverageValues)
                    .HasForeignKey(d => d.RegionId);
            });

            modelBuilder.Entity<DepartmentAverageValueByYear>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.Year });

                entity.ToView("department_avg_value_by_year");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.Region)
                    .WithMany(r => r.DepartmentAverageValuesByYear)
                    .HasForeignKey(d => d.RegionId);
            });

            modelBuilder.Entity<TownAverageValue>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type });

                entity.ToView("town_avg_value");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(t => t.Department)
                    .WithMany(d => d.TownAverageValues)
                    .HasForeignKey(t => t.DepartmentId);
            });

            modelBuilder.Entity<TownAverageValueByYear>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.Year });

                entity.ToView("town_avg_value_by_year");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(t => t.Department)
                    .WithMany(d => d.TownAverageValuesByYear)
                    .HasForeignKey(t => t.DepartmentId);
            });

            modelBuilder.Entity<SectionAverageValue>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type });

                entity.ToView("section_avg_value");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.TownId).HasColumnName("town_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(s => s.Town)
                    .WithMany(t => t.SectionAverageValues)
                    .HasForeignKey(s => s.TownId);
            });

            modelBuilder.Entity<SectionAverageValueByYear>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.Year });

                entity.ToView("section_avg_value_by_year");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.TownId).HasColumnName("town_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(s => s.Town)
                    .WithMany(t => t.SectionAverageValuesByYear)
                    .HasForeignKey(s => s.TownId);
            });

            modelBuilder.Entity<Place>(entity =>
            {
                entity.HasKey(e => new { e.Type, e.Id });

                entity.ToView("place");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.SearchName).HasColumnName("search_name");

                entity.Property(e => e.PostCode).HasColumnName("post_code");

                entity.Property(e => e.ParentType).HasColumnName("parent_type");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.Geometry).HasColumnName("geometry");

                entity.HasOne(p => p.Parent)
                    .WithMany()
                    .HasForeignKey(p => new { p.ParentType, p.ParentId });
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("message");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.Email).HasColumnName("email").IsRequired();

                entity.Property(e => e.Subject).HasColumnName("subject").IsRequired();

                entity.Property(e => e.Content).HasColumnName("content").IsRequired();

                entity.Property(e => e.SentOn).HasColumnName("sent_on");

                entity.Property(e => e.IsArchived).HasColumnName("is_archived").HasDefaultValue(false);
            });

            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.ToTable(name:"user");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            });

            modelBuilder.Entity<AdminRole>(entity =>
            {
                entity.ToTable(name: "role");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            });

            modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.ToTable("user_claim");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            });

            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable("user_login");
            });

            modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.ToTable("role_claim");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            });

            modelBuilder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.ToTable("user_role");
            });

            modelBuilder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.ToTable("user_token");
            });
        }
    }
}
