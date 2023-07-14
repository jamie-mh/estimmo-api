// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using NetTopologySuite.Geometries;

namespace Estimmo.Data.Entities
{
    public class SaidPlace
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TownId { get; set; }
        public string PostCode { get; set; }
        public Point Coordinates { get; set; }

        public virtual Town Town { get; set; }
    }
}
