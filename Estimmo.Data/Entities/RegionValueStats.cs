// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Estimmo.Data.Entities
{
    public class RegionValueStats : IValueStats
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
        public double? StandardDeviation { get; set; }

        public virtual Region Region { get; set; }
    }
}
