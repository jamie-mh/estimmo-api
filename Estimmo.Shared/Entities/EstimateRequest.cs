// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data.Entities;
using NetTopologySuite.Geometries;

namespace Estimmo.Shared.Entities
{
    public class EstimateRequest
    {
        public Point Coordinates { get; set; }
        public PropertyType PropertyType { get; set; }
        public int Rooms { get; set; }
        public int BuildingSurfaceArea { get; set; }
        public int LandSurfaceArea { get; set; }
    }
}
