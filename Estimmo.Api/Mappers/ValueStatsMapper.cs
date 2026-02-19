// Copyright (C) 2025 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data.Entities;
using Riok.Mapperly.Abstractions;
using System.Collections.Generic;

namespace Estimmo.Api.Mappers;

[Mapper]
public static partial class ValueStatsMapper
{
    [UserMapping]
    [MapDerivedType<RegionValueStats, IValueStats>]
    [MapDerivedType<DepartmentValueStats, IValueStats>]
    [MapDerivedType<TownValueStats, IValueStats>]
    [MapDerivedType<SectionValueStats, IValueStats>]
    private static KeyValuePair<short, double> MapToKeyValue(IValueStats stats) => new((short) stats.Type, stats.Average);

    public static partial Dictionary<short, double> MapToDictionary(IEnumerable<IValueStats> stats);

    [UserMapping]
    [MapDerivedType<RegionValueStatsByYear, IValueStats>]
    [MapDerivedType<DepartmentValueStatsByYear, IValueStats>]
    [MapDerivedType<TownValueStatsByYear, IValueStats>]
    [MapDerivedType<SectionValueStatsByYear, IValueStats>]
    private static KeyValuePair<short, double> MapToKeyValue(IValueStatsByYear stats) => new((short) stats.Type, stats.Average);
}
