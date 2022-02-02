using AutoMapper;
using Estimmo.Api.Entities.Json;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System.Collections.Generic;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class TownsTypeConverter : ITypeConverter<IEnumerable<Town>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<Town> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var feature in source)
            {
                var attributes = new AttributesTable
                {
                    { "id", feature.Id },
                    { "name", feature.Name },
                    { "averageValues", context.Mapper.Map<IEnumerable<JsonAverageValue>>(feature.AverageValues) }
                };

                collection.Add(new Feature(feature.Geometry, attributes));
            }

            return collection;
        }
    }
}
