using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class Coordinates
    {
        [Required]
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [Required]
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
