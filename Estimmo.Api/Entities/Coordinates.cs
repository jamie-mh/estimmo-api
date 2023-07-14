// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

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
