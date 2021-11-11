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
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Geometry Geometry { get; set; }

        public ICollection<Department> Departments { get; set; }
        public ICollection<DepartmentAverageValue> DepartmentAverageValues { get; set; }
    }
}
