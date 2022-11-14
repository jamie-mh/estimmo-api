using Estimmo.Api.JsonConverters;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class FeaturesWithValues
    {
        [JsonPropertyName("averageValues")]
        [JsonConverter(typeof(AverageValuesJsonConverter))]
        public IEnumerable<IAverageValue> AverageValues { get; set; }

        [JsonPropertyName("averageValuesByYear")]
        [JsonConverter(typeof(AverageValuesByYearJsonConverter))]
        public IEnumerable<IAverageValueByYear> AverageValuesByYear { get; set; }

        [JsonPropertyName("geojson")]
        public FeatureCollection GeoJson { get; set; }
    }
}
