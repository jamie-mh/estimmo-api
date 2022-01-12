using Estimmo.Data.Entities;

namespace Estimmo.Api.Models.Features
{
    public class TownFeature
    {
        public Town Town { get; set; }
        public TownAverageValue AverageValue { get; set; }
    }
}
