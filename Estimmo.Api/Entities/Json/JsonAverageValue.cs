using Estimmo.Data.Entities;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities.Json
{
    public class JsonAverageValue
    {
        [JsonPropertyName("type")]
        public PropertyType Type { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
}
