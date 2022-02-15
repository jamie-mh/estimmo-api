using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class DetailedPlace : SimplePlace
    {
        [JsonPropertyName("hierarchy")]
        public List<SimplePlace> Hierarchy { get; set; }
    }
}
