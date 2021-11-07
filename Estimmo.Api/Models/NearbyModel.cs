using System;
using System.ComponentModel.DataAnnotations;

namespace Estimmo.Api.Models
{
    public class NearbyModel
    {
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        [Range(0d, Double.MaxValue)]
        public double Radius { get; set; }
    }
}
