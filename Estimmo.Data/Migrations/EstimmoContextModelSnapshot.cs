﻿// <auto-generated />
using System;
using Estimmo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Estimmo.Data.Migrations
{
    [DbContext(typeof(EstimmoContext))]
    partial class EstimmoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("postgis")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Estimmo.Data.Entities.Parcel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<Geometry>("Geometry")
                        .IsRequired()
                        .HasColumnType("geography")
                        .HasColumnName("geometry");

                    b.Property<int>("Number")
                        .HasColumnType("integer")
                        .HasColumnName("number");

                    b.Property<string>("Prefix")
                        .HasColumnType("text")
                        .HasColumnName("prefix");

                    b.Property<string>("SectionCode")
                        .HasColumnType("text")
                        .HasColumnName("section_code");

                    b.Property<string>("TownId")
                        .HasColumnType("text")
                        .HasColumnName("town_id");

                    b.HasKey("Id");

                    b.HasIndex("TownId");

                    b.ToTable("parcel");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.PropertySale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

                    b.Property<int>("BuildingSurfaceArea")
                        .HasColumnType("integer")
                        .HasColumnName("building_surface_area");

                    b.Property<Point>("Coordinates")
                        .HasColumnType("geography")
                        .HasColumnName("coodinates");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date")
                        .HasColumnName("date");

                    b.Property<int>("LandSurfaceArea")
                        .HasColumnType("integer")
                        .HasColumnName("land_surface_area");

                    b.Property<string>("ParcelId")
                        .HasColumnType("text");

                    b.Property<string>("PostCode")
                        .HasColumnType("text")
                        .HasColumnName("post_code");

                    b.Property<short>("RoomCount")
                        .HasColumnType("smallint")
                        .HasColumnName("room_count");

                    b.Property<string>("StreetName")
                        .HasColumnType("text")
                        .HasColumnName("street_name");

                    b.Property<short?>("StreetNumber")
                        .HasColumnType("smallint")
                        .HasColumnName("street_number");

                    b.Property<string>("StreetNumberSuffix")
                        .HasColumnType("text")
                        .HasColumnName("street_number_suffix");

                    b.Property<int>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<decimal>("Value")
                        .HasColumnType("money")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.HasIndex("Coordinates")
                        .HasDatabaseName("property_sale_coordinates_idx")
                        .HasMethod("gist");

                    b.ToTable("property_sale");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Section", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<Geometry>("Geometry")
                        .IsRequired()
                        .HasColumnType("geography")
                        .HasColumnName("geometry");

                    b.Property<string>("Prefix")
                        .HasColumnType("text")
                        .HasColumnName("prefix");

                    b.Property<string>("TownId")
                        .HasColumnType("text")
                        .HasColumnName("town_id");

                    b.HasKey("Id");

                    b.HasIndex("TownId");

                    b.ToTable("section");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Town", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<Geometry>("Geometry")
                        .IsRequired()
                        .HasColumnType("geography")
                        .HasColumnName("geometry");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("town");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Parcel", b =>
                {
                    b.HasOne("Estimmo.Data.Entities.Section", "Section")
                        .WithMany("Parcels")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Estimmo.Data.Entities.Town", "Town")
                        .WithMany()
                        .HasForeignKey("TownId");

                    b.Navigation("Section");

                    b.Navigation("Town");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Section", b =>
                {
                    b.HasOne("Estimmo.Data.Entities.Town", "Town")
                        .WithMany("Sections")
                        .HasForeignKey("TownId");

                    b.Navigation("Town");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Section", b =>
                {
                    b.Navigation("Parcels");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Town", b =>
                {
                    b.Navigation("Sections");
                });
#pragma warning restore 612, 618
        }
    }
}
