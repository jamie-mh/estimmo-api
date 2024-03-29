// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using AutoMapper;
using Estimmo.Api.Extension;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class RegionsTypeConverter : ITypeConverter<IEnumerable<Region>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<Region> source,
            NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var region in source)
            {
                var attributes = new AttributesTable
                {
                    { "featureId", Math.Abs(region.Id.GetHashCode()) },
                    { "id", region.Id },
                    { "name", region.Name },
                    {
                        "medianValues",
                        context.Mapper.Map<Dictionary<short, double>>(region.ValueStatsByYear.Any()
                            ? region.ValueStatsByYear
                            : region.ValueStats)
                    }
                };

                collection.Add(new Feature(region.Geometry, attributes));
            }

            collection.CalculateBoundingBox();
            return collection;
        }
    }
}
