using Estimmo.Data.Entities;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class SimplePlace
    {
        [JsonPropertyName("type")]
        public PlaceType Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("shortName")]
        public string ShortName { get; set; }

        [JsonPropertyName("postCode")]
        public string PostCode { get; set; }
    }
}
