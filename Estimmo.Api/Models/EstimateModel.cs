// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Api.Entities;
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
        [Range(1, int.MaxValue)]
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

        [Required] [JsonPropertyName("type")] public PropertyType PropertyType { get; set; }
    }
}
