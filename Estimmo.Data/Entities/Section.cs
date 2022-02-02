using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Section
    {
        public Section()
        {
            PropertySales = new HashSet<PropertySale>();
            AverageValues = new HashSet<SectionAverageValue>();
        }

        public string Id { get; set; }
        public string TownId { get; set; }
        public string Prefix { get; set; }
        public string Code { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Town Town { get; set; }
        public virtual ICollection<PropertySale> PropertySales { get; set; }
        public virtual ICollection<SectionAverageValue> AverageValues { get; set; }
    }
}
