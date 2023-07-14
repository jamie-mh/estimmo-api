// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class PlaceWithHierarchy : SimplePlace
    {
        [JsonPropertyName("parent")]
        public PlaceWithHierarchy Parent { get; set; }
    }
}
