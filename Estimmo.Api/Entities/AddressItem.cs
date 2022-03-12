using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class AddressItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("postCode")]
        public string PostCode { get; set; }

        [JsonPropertyName("coordinates")]
        public Coordinates Coordinates { get; set; }
    }
}
