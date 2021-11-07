using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Estimmo.Api.Models
{
    public class NearbyModel
    {
        [FromQuery]
        [Required]
        public double Latitude { get; set; }

        [FromQuery]
        [Required]
        public double Longitude { get; set; }

        [FromQuery]
        [Required]
        [Range(0d, Double.MaxValue)]
        public double Radius { get; set; }
    }
}
