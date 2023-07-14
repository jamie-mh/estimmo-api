// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using NetTopologySuite.Geometries;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class RootPlace : SimplePlace
    {
        [JsonPropertyName("parent")]
        public PlaceWithHierarchy Parent { get; set; }
        
        [JsonPropertyName("geometry")]
        public Geometry Geometry { get; set; }
    }
}
