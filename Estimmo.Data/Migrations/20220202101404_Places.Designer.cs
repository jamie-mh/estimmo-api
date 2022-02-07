﻿// <auto-generated />
using System;
using Estimmo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Estimmo.Data.Migrations
{
    [DbContext(typeof(EstimmoContext))]
    [Migration("20220202101404_Places")]
    partial class Places
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "postgis");
            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "unaccent");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Estimmo.Data.Entities.Department", b =>
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

                    b.Property<string>("RegionId")
                        .HasColumnType("text")
                        .HasColumnName("region_id");

                    b.HasKey("Id");

                    b.HasIndex("Geometry");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("Geometry"), "gist");

                    b.HasIndex("RegionId");

                    b.ToTable("department", (string)null);
                });

            modelBuilder.Entity("Estimmo.Data.Entities.DepartmentAverageValue", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<string>("RegionId")
                        .HasColumnType("text")
                        .HasColumnName("region_id");

                    b.Property<double>("Value")
                        .HasColumnType("double precision")
                        .HasColumnName("value");

                    b.HasKey("Id", "Type");

                    b.HasIndex("RegionId");

                    b.ToView("department_avg_value");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.PropertySale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<int>("BuildingSurfaceArea")
                        .HasColumnType("integer")
                        .HasColumnName("building_surface_area");

                    b.Property<Point>("Coordinates")
                        .IsRequired()
                        .HasColumnType("geography")
                        .HasColumnName("coodinates");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date")
                        .HasColumnName("date");

                    b.Property<int>("LandSurfaceArea")
                        .HasColumnType("integer")
                        .HasColumnName("land_surface_area");

                    b.Property<string>("PostCode")
                        .HasColumnType("text")
                        .HasColumnName("post_code");

                    b.Property<short>("RoomCount")
                        .HasColumnType("smallint")
                        .HasColumnName("room_count");

                    b.Property<string>("SectionId")
                        .HasColumnType("text")
                        .HasColumnName("section_id");

                    b.Property<string>("StreetName")
                        .IsRequired()
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

                    b.HasIndex("Coordinates");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("Coordinates"), "gist");

                    b.HasIndex("SectionId");

                    b.ToTable("property_sale", (string)null);
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Region", b =>
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

                    b.HasIndex("Geometry");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("Geometry"), "gist");

                    b.ToTable("region", (string)null);
                });

            modelBuilder.Entity("Estimmo.Data.Entities.RegionAverageValue", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<double>("Value")
                        .HasColumnType("double precision")
                        .HasColumnName("value");

                    b.HasKey("Id", "Type");

                    b.ToView("region_avg_value");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Section", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<Geometry>("Geometry")
                        .IsRequired()
                        .HasColumnType("geography")
                        .HasColumnName("geometry");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("prefix");

                    b.Property<string>("TownId")
                        .HasColumnType("text")
                        .HasColumnName("town_id");

                    b.HasKey("Id");

                    b.HasIndex("Geometry");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("Geometry"), "gist");

                    b.HasIndex("TownId");

                    b.ToTable("section", (string)null);
                });

            modelBuilder.Entity("Estimmo.Data.Entities.SectionAverageValue", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<string>("TownId")
                        .HasColumnType("text")
                        .HasColumnName("town_id");

                    b.Property<double>("Value")
                        .HasColumnType("double precision")
                        .HasColumnName("value");

                    b.HasKey("Id", "Type");

                    b.HasIndex("TownId");

                    b.ToView("section_avg_value");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Town", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("DepartmentId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("department_id");

                    b.Property<Geometry>("Geometry")
                        .IsRequired()
                        .HasColumnType("geography")
                        .HasColumnName("geometry");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("Geometry");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("Geometry"), "gist");

                    b.ToTable("town", (string)null);
                });

            modelBuilder.Entity("Estimmo.Data.Entities.TownAverageValue", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<string>("DepartmentId")
                        .HasColumnType("text")
                        .HasColumnName("department_id");

                    b.Property<double>("Value")
                        .HasColumnType("double precision")
                        .HasColumnName("value");

                    b.HasKey("Id", "Type");

                    b.HasIndex("DepartmentId");

                    b.ToView("town_avg_value");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Department", b =>
                {
                    b.HasOne("Estimmo.Data.Entities.Region", "Region")
                        .WithMany("Departments")
                        .HasForeignKey("RegionId");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.DepartmentAverageValue", b =>
                {
                    b.HasOne("Estimmo.Data.Entities.Region", "Region")
                        .WithMany("DepartmentAverageValues")
                        .HasForeignKey("RegionId");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.PropertySale", b =>
                {
                    b.HasOne("Estimmo.Data.Entities.Section", "Section")
                        .WithMany("PropertySales")
                        .HasForeignKey("SectionId");

                    b.Navigation("Section");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Section", b =>
                {
                    b.HasOne("Estimmo.Data.Entities.Town", "Town")
                        .WithMany("Sections")
                        .HasForeignKey("TownId");

                    b.Navigation("Town");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.SectionAverageValue", b =>
                {
                    b.HasOne("Estimmo.Data.Entities.Town", "Town")
                        .WithMany("SectionAverageValues")
                        .HasForeignKey("TownId");

                    b.Navigation("Town");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Town", b =>
                {
                    b.HasOne("Estimmo.Data.Entities.Department", "Department")
                        .WithMany("Towns")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.TownAverageValue", b =>
                {
                    b.HasOne("Estimmo.Data.Entities.Department", "Department")
                        .WithMany("TownAverageValues")
                        .HasForeignKey("DepartmentId");

                    b.Navigation("Department");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Department", b =>
                {
                    b.Navigation("TownAverageValues");

                    b.Navigation("Towns");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Region", b =>
                {
                    b.Navigation("DepartmentAverageValues");

                    b.Navigation("Departments");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Section", b =>
                {
                    b.Navigation("PropertySales");
                });

            modelBuilder.Entity("Estimmo.Data.Entities.Town", b =>
                {
                    b.Navigation("SectionAverageValues");

                    b.Navigation("Sections");
                });
#pragma warning restore 612, 618
        }
    }
}