using AutoMapper;
using Estimmo.Api.Models.Features;
using NetTopologySuite.Features;
using System.Collections.Generic;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class TownsTypeConverter : ITypeConverter<IEnumerable<TownFeature>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<TownFeature> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var feature in source)
            {
                var attributes = new AttributesTable
                {
                    { "id", feature.Town.Id },
                    { "name", feature.Town.Name },
                    { "averageValue", feature.AverageValue?.Value }
                };

                collection.Add(new Feature(feature.Town.Geometry, attributes));
            }

            return collection;
        }
    }
}
