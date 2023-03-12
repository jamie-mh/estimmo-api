using AutoMapper;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class TownsTypeConverter : ITypeConverter<IEnumerable<Town>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<Town> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var town in source)
            {
                var attributes = new AttributesTable
                {
                    { "featureId", Math.Abs(town.Id.GetHashCode()) },
                    { "id", town.Id },
                    { "name", town.Name },
                    { "postCode", town.PostCode },
                    {
                        "medianValues",
                        context.Mapper.Map<Dictionary<short, double>>(town.ValueStatsByYear.Any()
                            ? town.ValueStatsByYear
                            : town.ValueStats)
                    }
                };

                collection.Add(new Feature(town.Geometry, attributes));
            }

            collection.CalculateBoundingBox();
            return collection;
        }
    }
}
