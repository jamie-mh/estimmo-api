// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using NetTopologySuite.Geometries;

namespace Estimmo.Data.Entities
{
    public class Address
    {
        public string Id { get; set; }
        public int? Number { get; set; }
        public string Suffix { get; set; }
        public string PostCode { get; set; }
        public string StreetId { get; set; }
        public Point Coordinates { get; set; }

        public virtual Street Street { get; set; }
    }
}
