using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class DetailedPlace : SimplePlace
    {
        [JsonPropertyName("parent")]
        public DetailedPlace Parent { get; set; }
    }
}
