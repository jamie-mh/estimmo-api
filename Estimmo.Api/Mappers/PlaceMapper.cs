// Copyright (C) 2025 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Api.Entities;
using Estimmo.Data.Entities;
using Riok.Mapperly.Abstractions;
using System.Linq;

namespace Estimmo.Api.Mappers;

[Mapper]
public static partial class PlaceMapper
{
    [MapperIgnoreSource(nameof(Place.SearchName))]
    [MapperIgnoreSource(nameof(Place.ParentType))]
    [MapperIgnoreSource(nameof(Place.ParentId))]
    [MapperIgnoreSource(nameof(Place.Parent))]
    [MapperIgnoreSource(nameof(Place.IsSearchable))]
    [MapperIgnoreSource(nameof(Place.Geometry))]
    public static partial SimplePlace MapToSimplePlace(Place place);

    public static partial IQueryable<SimplePlace> ProjectToSimplePlaces(this IQueryable<Place> places);

    [MapperIgnoreSource(nameof(Place.SearchName))]
    [MapperIgnoreSource(nameof(Place.ParentType))]
    [MapperIgnoreSource(nameof(Place.ParentId))]
    [MapperIgnoreSource(nameof(Place.IsSearchable))]
    public static partial RootPlace MapToRootPlace(Place place);

    [MapperIgnoreSource(nameof(Place.SearchName))]
    [MapperIgnoreSource(nameof(Place.ParentType))]
    [MapperIgnoreSource(nameof(Place.ParentId))]
    [MapperIgnoreSource(nameof(Place.IsSearchable))]
    [MapperIgnoreSource(nameof(Place.Geometry))]
    private static partial PlaceWithHierarchy MapToPlaceWithHierarchy(Place place);
}
