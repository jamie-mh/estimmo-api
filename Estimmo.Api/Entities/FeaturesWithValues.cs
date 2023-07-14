// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Api.JsonConverters;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class FeaturesWithValues
    {
        [JsonPropertyName("medianValues")]
        [JsonConverter(typeof(MedianValuesJsonConverter))]
        public IEnumerable<IValueStats> ValueStats { get; set; }

        [JsonPropertyName("medianValuesByYear")]
        [JsonConverter(typeof(MedianValuesByYearJsonConverter))]
        public IEnumerable<IValueStatsByYear> ValueStatsByYear { get; set; }

        [JsonPropertyName("geojson")]
        public FeatureCollection GeoJson { get; set; }
    }
}
