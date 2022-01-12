using Estimmo.Data.Entities;

namespace Estimmo.Api.Models.Features
{
    public class RegionFeature
    {
        public Region Region { get; set; }
        public RegionAverageValue AverageValue { get; set; }
    }
}
