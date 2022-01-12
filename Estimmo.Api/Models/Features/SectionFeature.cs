using Estimmo.Data.Entities;

namespace Estimmo.Api.Models.Features
{
    public class SectionFeature
    {
        public Section Section { get; set; }
        public SectionAverageValue AverageValue { get; set; }
    }
}
