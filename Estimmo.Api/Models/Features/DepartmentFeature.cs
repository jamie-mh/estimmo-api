using Estimmo.Data.Entities;

namespace Estimmo.Api.Models.Features
{
    public class DepartmentFeature
    {
        public Department Department { get; set; }
        public DepartmentAverageValue AverageValue { get; set; }
    }
}
