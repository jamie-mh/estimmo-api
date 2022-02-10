using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Models
{
    public class MessageModel
    {
        [Required]
        [MaxLength(100)]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(200)]
        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [Required]
        [MaxLength(5000)]
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
