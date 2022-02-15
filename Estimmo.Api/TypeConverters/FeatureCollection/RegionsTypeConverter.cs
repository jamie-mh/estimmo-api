using AutoMapper;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class RegionsTypeConverter : ITypeConverter<IEnumerable<Region>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<Region> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var region in source)
            {
                var attributes = new AttributesTable
                {
                    { "featureId", Math.Abs(region.Id.GetHashCode()) },
                    { "id", region.Id },
                    { "name", region.Name },
                    { "averageValues", region.AverageValues.ToDictionary(r => (int) r.Type, r => r.Value) }
                };

                collection.Add(new Feature(region.Geometry, attributes));
            }

            collection.CalculateBoundingBox();
            return collection;
        }
    }
}
