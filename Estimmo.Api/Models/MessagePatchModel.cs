using System.Text.Json.Serialization;

namespace Estimmo.Api.Models
{
    public class MessagePatchModel
    {
        [JsonPropertyName("isArchived")]
        public bool IsArchived { get; set; }
    }
}
