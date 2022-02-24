using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Region
    {
        public Region()
        {
            Departments = new HashSet<Department>();
            DepartmentAverageValues = new HashSet<DepartmentAverageValue>();
            DepartmentAverageValuesByYear = new HashSet<DepartmentAverageValueByYear>();
            AverageValues = new HashSet<RegionAverageValue>();
            AverageValuesByYear = new HashSet<RegionAverageValueByYear>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Geometry Geometry { get; set; }

        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<DepartmentAverageValue> DepartmentAverageValues { get; set; }
        public virtual ICollection<DepartmentAverageValueByYear> DepartmentAverageValuesByYear { get; set; }
        public virtual ICollection<RegionAverageValue> AverageValues { get; set; }
        public virtual ICollection<RegionAverageValueByYear> AverageValuesByYear { get; set; }
    }
}
