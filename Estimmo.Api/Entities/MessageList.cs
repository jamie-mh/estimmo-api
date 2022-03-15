using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class MessageList
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("messages")]
        public IEnumerable<SimpleMessage> Messages { get; set; }
    }
}
