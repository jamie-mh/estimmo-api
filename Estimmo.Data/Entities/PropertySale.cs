// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using NetTopologySuite.Geometries;
using System;

namespace Estimmo.Data.Entities
{
    public class PropertySale
    {
        public string Hash { get; set; }
        public DateTime Date { get; set; }
        public short? StreetNumber { get; set; }
        public string StreetNumberSuffix { get; set; }
        public string StreetName { get; set; }
        public string PostCode { get; set; }
        public PropertyType Type { get; set; }
        public int BuildingSurfaceArea { get; set; }
        public int LandSurfaceArea { get; set; }
        public short RoomCount { get; set; }
        public decimal Value { get; set; }
        public string SectionId { get; set; }
        public Point Coordinates { get; set; }

        public virtual Section Section { get; set; }
    }
}
