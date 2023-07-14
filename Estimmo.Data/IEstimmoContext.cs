using Estimmo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Estimmo.Data
{
    public interface IEstimmoContext
    {
        public DbSet<Region> Regions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<PropertySale> PropertySales { get; set; }
        public DbSet<FranceValueStats> FranceValueStats { get; set; }
        public DbSet<FranceValueStatsByYear> FranceValueStatsByYear { get; set; }
        public DbSet<RegionValueStats> RegionValueStats { get; set; }
        public DbSet<RegionValueStatsByYear> RegionValueStatsByYear { get; set; }
        public DbSet<DepartmentValueStats> DepartmentValueStats { get; set; }
        public DbSet<DepartmentValueStatsByYear> DepartmentValueStatsByYear { get; set; }
        public DbSet<TownValueStats> TownValueStats { get; set; }
        public DbSet<TownValueStatsByYear> TownValueStatsByYear { get; set; }
        public DbSet<SectionValueStats> SectionValueStats { get; set; }
        public DbSet<SectionValueStatsByYear> SectionValueStatsByYear { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<SaidPlace> SaidPlaces { get; set; }
    }
}
