using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class PlaceWithHierarchy : SimplePlace
    {
        [JsonPropertyName("parent")]
        public PlaceWithHierarchy Parent { get; set; }
    }
}
