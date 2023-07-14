// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Estimmo.Data.Entities
{
    public interface IValueStats
    {
        public PropertyType Type { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
        public double? StandardDeviation { get; set; }
    }
}
