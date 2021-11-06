using NetTopologySuite.Geometries;
using System;

namespace Estimmo.Data.Entities
{
    public class PropertySale
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public short? StreetNumber { get; set; }
        public string StreetNumberSuffix { get; set; }
        public string StreetName { get; set; }
        public string PostCode { get; set; }
        public PropertyType Type { get; set; }
        public int BuildingSurfaceArea { get; set; }
        public int LandSurfaceArea { get; set; }
        public short RoomCount { get; set; }
        public decimal Value { get; set; }
        public string ParcelId { get; set; }
        public Point Coordinates { get; set; }
    }
}
