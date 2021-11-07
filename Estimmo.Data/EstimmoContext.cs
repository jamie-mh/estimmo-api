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

        public virtual DbSet<PropertySale> PropertySales { get; set; }
        public virtual DbSet<Town> Towns { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Parcel> Parcels { get; set; }

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

            modelBuilder.Entity<PropertySale>(entity =>
            {
                entity.ToTable("property_sale");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Date).HasColumnName("date").HasColumnType("date");

                entity.Property(e => e.StreetNumber).HasColumnName("street_number");

                entity.Property(e => e.StreetNumberSuffix).HasColumnName("street_number_suffix");

                entity.Property(e => e.StreetName).HasColumnName("street_name");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("smallint")
                    .HasConversion(v => (int) v, v => (PropertyType) v);

                entity.Property(e => e.PostCode).HasColumnName("post_code");

                entity.Property(e => e.BuildingSurfaceArea).HasColumnName("building_surface_area");

                entity.Property(e => e.LandSurfaceArea).HasColumnName("land_surface_area");

                entity.Property(e => e.RoomCount).HasColumnName("room_count");

                entity.Property(e => e.Value).HasColumnName("value").HasColumnType("money");

                entity.Property(e => e.Coordinates).HasColumnName("coodinates").HasColumnType("geography");

                entity.HasIndex(e => e.Coordinates).HasMethod("gist").HasDatabaseName("property_sale_coordinates_idx");
            });

            modelBuilder.Entity<Town>(entity =>
            {
                entity.ToTable("town");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name").IsRequired();

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.ToTable("section");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TownId).HasColumnName("town_id");

                entity.Property(e => e.Prefix).HasColumnName("prefix");

                entity.Property(e => e.Code).HasColumnName("code");

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.HasOne(s => s.Town)
                    .WithMany(t => t.Sections)
                    .HasForeignKey(s => s.TownId);
            });

            modelBuilder.Entity<Parcel>(entity =>
            {
                entity.ToTable("parcel");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SectionId).HasColumnName("section_id");

                entity.Property(e => e.TownId).HasColumnName("town_id");

                entity.Property(e => e.Prefix).HasColumnName("prefix");

                entity.Property(e => e.SectionCode).HasColumnName("section_code");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.Geometry).HasColumnName("geometry").HasColumnType("geography").IsRequired();

                entity.HasOne(p => p.Section)
                    .WithMany(s => s.Parcels)
                    .HasForeignKey(p => p.SectionId);

                entity.HasOne(p => p.Town)
                    .WithMany(t => t.Parcels)
                    .HasForeignKey(p => p.TownId);
            });
        }
    }
}