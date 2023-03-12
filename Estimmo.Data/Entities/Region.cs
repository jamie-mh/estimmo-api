using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Region
    {
        public Region()
        {
            Departments = new HashSet<Department>();
            DepartmentValueStats = new HashSet<DepartmentValueStats>();
            DepartmentValueStatsByYear = new HashSet<DepartmentValueStatsByYear>();
            ValueStats = new HashSet<RegionValueStats>();
            ValueStatsByYear = new HashSet<RegionValueStatsByYear>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Geometry Geometry { get; set; }

        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<DepartmentValueStats> DepartmentValueStats { get; set; }
        public virtual ICollection<DepartmentValueStatsByYear> DepartmentValueStatsByYear { get; set; }
        public virtual ICollection<RegionValueStats> ValueStats { get; set; }
        public virtual ICollection<RegionValueStatsByYear> ValueStatsByYear { get; set; }
    }
}
