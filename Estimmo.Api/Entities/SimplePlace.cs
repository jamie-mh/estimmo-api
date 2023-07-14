// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data.Entities;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class SimplePlace
    {
        [JsonPropertyName("type")]
        public PlaceType Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("shortName")]
        public string ShortName { get; set; }

        [JsonPropertyName("postCode")]
        public string PostCode { get; set; }
    }
}
