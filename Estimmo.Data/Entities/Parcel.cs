using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Parcel
    {
        public Parcel()
        {
            PropertySales = new HashSet<PropertySale>();
        }

        public string Id { get; set; }
        public string SectionId { get; set; }
        public string TownId { get; set; }
        public string Prefix { get; set; }
        public string SectionCode { get; set; }
        public int Number { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Town Town { get; set; }
        public virtual Section Section { get; set; }
        public ICollection<PropertySale> PropertySales { get; set; }
    }
}
