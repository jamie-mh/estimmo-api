using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Models
{
    public class LoginModel
    {
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
