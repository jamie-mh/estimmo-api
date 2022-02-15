using AutoMapper;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class SectionsTypeConverter : ITypeConverter<IEnumerable<Section>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<Section> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var section in source)
            {
                var attributes = new AttributesTable
                {
                    { "featureId", Math.Abs(section.Id.GetHashCode()) },
                    { "id", section.Id },
                    { "averageValues", section.AverageValues.ToDictionary(r => (int) r.Type, r => r.Value) }
                };

                collection.Add(new Feature(section.Geometry, attributes));
            }

            collection.CalculateBoundingBox();
            return collection;
        }
    }
}
