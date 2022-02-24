using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Department
    {
        public Department()
        {
            Towns = new HashSet<Town>();
            TownAverageValues = new HashSet<TownAverageValue>();
            TownAverageValuesByYear = new HashSet<TownAverageValueByYear>();
            AverageValues = new HashSet<DepartmentAverageValue>();
            AverageValuesByYear = new HashSet<DepartmentAverageValueByYear>();
        }

        public string Id { get; set; }
        public string RegionId { get; set; }
        public string Name { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Region Region { get; set; }
        public virtual ICollection<Town> Towns { get; set; }
        public virtual ICollection<TownAverageValue> TownAverageValues { get; set; }
        public virtual ICollection<TownAverageValueByYear> TownAverageValuesByYear { get; set; }
        public virtual ICollection<DepartmentAverageValue> AverageValues { get; set; }
        public virtual ICollection<DepartmentAverageValueByYear> AverageValuesByYear { get; set; }
    }
}
