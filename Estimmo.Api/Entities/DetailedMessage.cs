using System;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class DetailedMessage
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("sent_on")]
        public DateTime SentOn { get; set; }

        [JsonPropertyName("isArchived")]
        public bool IsArchived { get; set; }
    }
}
