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
    public class DepartmentsTypeConverter : ITypeConverter<IEnumerable<Department>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<Department> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var department in source)
            {
                var attributes = new AttributesTable
                {
                    { "featureId", Math.Abs(department.Id.GetHashCode()) },
                    { "id", department.Id },
                    { "regionId", department.RegionId },
                    { "name", department.Name },
                    {
                        "medianValues",
                        context.Mapper.Map<Dictionary<short, double>>(department.ValueStatsByYear.Any()
                            ? department.ValueStatsByYear
                            : department.ValueStats)
                    }
                };

                collection.Add(new Feature(department.Geometry, attributes));
            }

            collection.CalculateBoundingBox();
            return collection;
        }
    }
}
