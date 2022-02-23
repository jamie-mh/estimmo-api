using NetTopologySuite.Features;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class CadastreItem
    {
        [JsonPropertyName("averageValues")]
        public Dictionary<int, double> AverageValues { get; set; }

        [JsonPropertyName("geojson")]
        public FeatureCollection GeoJson { get; set; }
    }
}
