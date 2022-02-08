using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Town
    {
        public Town()
        {
            Sections = new HashSet<Section>();
            SectionAverageValues = new HashSet<SectionAverageValue>();
            AverageValues = new HashSet<TownAverageValue>();
        }

        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string Name { get; set; }
        public string PostCode { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
        public virtual ICollection<SectionAverageValue> SectionAverageValues { get; set; }
        public virtual ICollection<TownAverageValue> AverageValues { get; set; }
    }
}
