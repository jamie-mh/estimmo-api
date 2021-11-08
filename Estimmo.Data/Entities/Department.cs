using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Department
    {
        public Department()
        {
            Towns = new HashSet<Town>();
        }

        public string Id { get; set; }
        public string RegionId { get; set; }
        public string Name { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Region Region { get; set; }
        public virtual ICollection<Town> Towns { get; set; }
    }
}
