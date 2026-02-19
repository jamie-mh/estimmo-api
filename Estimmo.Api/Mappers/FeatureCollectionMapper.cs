// Copyright (C) 2025 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Api.Extension;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Estimmo.Api.Mappers;

[Mapper]
public static partial class FeatureCollectionMapper
{
    [UserMapping]
    public static FeatureCollection MapToFeatureCollection(IEnumerable<Region> regions)
    {
        var collection = new FeatureCollection();

        foreach (var region in regions)
        {
            var attributes = new AttributesTable
            {
                { "featureId", Math.Abs(region.Id.GetHashCode()) },
                { "id", region.Id },
                { "name", region.Name },
                {
                    "medianValues", ValueStatsMapper.MapToDictionary(region.ValueStatsByYear.Any()
                        ? region.ValueStatsByYear
                        : region.ValueStats)
                }
            };

            collection.Add(new Feature(region.Geometry, attributes));
        }

        collection.CalculateBoundingBox();
        return collection;
    }

    [UserMapping]
    public static FeatureCollection MapToFeatureCollection(IEnumerable<Department> departments)
    {
        var collection = new FeatureCollection();

        foreach (var department in departments)
        {
            var attributes = new AttributesTable
            {
                { "featureId", Math.Abs(department.Id.GetHashCode()) },
                { "id", department.Id },
                { "regionId", department.RegionId },
                { "name", department.Name },
                {
                    "medianValues", ValueStatsMapper.MapToDictionary(department.ValueStatsByYear.Any()
                        ? department.ValueStatsByYear
                        : department.ValueStats)
                }
            };

            collection.Add(new Feature(department.Geometry, attributes));
        }

        collection.CalculateBoundingBox();
        return collection;
    }

    [UserMapping]
    public static FeatureCollection MapToFeatureCollection(IEnumerable<Town> towns)
    {
        var collection = new FeatureCollection();

        foreach (var town in towns)
        {
            var attributes = new AttributesTable
            {
                { "featureId", Math.Abs(town.Id.GetHashCode()) },
                { "id", town.Id },
                { "name", town.Name },
                { "postCode", town.PostCode },
                {
                    "medianValues", ValueStatsMapper.MapToDictionary(town.ValueStatsByYear.Any()
                        ? town.ValueStatsByYear
                        : town.ValueStats)
                }
            };

            collection.Add(new Feature(town.Geometry, attributes));
        }

        collection.CalculateBoundingBox();
        return collection;
    }

    [UserMapping]
    public static FeatureCollection MapToFeatureCollection(IEnumerable<Section> sections)
    {
        var collection = new FeatureCollection();

        foreach (var section in sections)
        {
            var attributes = new AttributesTable
            {
                { "featureId", Math.Abs(section.Id.GetHashCode()) },
                { "id", section.Id },
                {
                    "medianValues", ValueStatsMapper.MapToDictionary(section.ValueStatsByYear.Any()
                        ? section.ValueStatsByYear
                        : section.ValueStats)
                }
            };

            collection.Add(new Feature(section.Geometry, attributes));
        }

        collection.CalculateBoundingBox();
        return collection;
    }

    [UserMapping]
    public static FeatureCollection MapToFeatureCollection(IEnumerable<PropertySale> sales)
    {
        var collection = new FeatureCollection();

        foreach (var sale in sales)
        {
            var id = HashCode.Combine(sale.Date, sale.StreetNumber, sale.StreetNumberSuffix, sale.StreetNumber, sale.Value);

            var attributes = new AttributesTable
            {
                { "featureId", Math.Abs(id) },
                { "date", sale.Date },
                { "streetNumber", sale.StreetNumber },
                { "streetNumberSuffix", sale.StreetNumberSuffix },
                { "streetName", sale.StreetName },
                { "postCode", sale.PostCode },
                { "type", sale.Type },
                { "buildingSurfaceArea", sale.BuildingSurfaceArea },
                { "landSurfaceArea", sale.LandSurfaceArea },
                { "roomCount", sale.RoomCount },
                { "value", sale.Value }
            };

            collection.CalculateBoundingBox();
            collection.Add(new Feature(sale.Coordinates, attributes));
        }

        return collection;
    }
}
