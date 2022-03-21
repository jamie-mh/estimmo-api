using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class DetailedMessage : SimpleMessage
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
