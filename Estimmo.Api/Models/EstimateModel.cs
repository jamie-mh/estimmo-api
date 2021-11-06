using Estimmo.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Models
{
    public class EstimateModel
    {
        [Required]
        [JsonPropertyName("coordinates")]
        public Coordinates PropertyCoordinates { get; set; }

        [Required]
        [JsonPropertyName("rooms")]
        public int Rooms { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [JsonPropertyName("buildingSurfaceArea")]
        public int BuildingSurfaceArea { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [JsonPropertyName("landSurfaceArea")]
        public int LandSurfaceArea { get; set; }

        [Required]
        [JsonPropertyName("type")]
        public PropertyType PropertyType { get; set; }

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
}