// Copyright (C) 2025 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Api.Models;
using Estimmo.Shared.Entities;
using NetTopologySuite.Geometries;
using Riok.Mapperly.Abstractions;
using Coordinates = Estimmo.Api.Entities.Coordinates;

namespace Estimmo.Api.Mappers;

[Mapper]
public static partial class EstimateMapper
{
    [MapProperty(nameof(EstimateModel.PropertyCoordinates), nameof(EstimateRequest.Coordinates), Use = nameof(MapToPoint))]
    public static partial EstimateRequest MapToRequest(EstimateModel model);

    [UserMapping]
    private static Point MapToPoint(Coordinates coordinates) => new(coordinates.Longitude, coordinates.Latitude);
}
