// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Estimmo.Data.Entities
{
    public interface IValueStatsByYear : IValueStats
    {
        public short Year { get; set; }
    }
}
