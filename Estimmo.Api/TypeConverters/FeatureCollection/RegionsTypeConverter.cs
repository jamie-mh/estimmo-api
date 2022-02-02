using AutoMapper;
using Estimmo.Api.Entities.Json;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System.Collections.Generic;

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
                    { "id", region.Id },
                    { "name", region.Name },
                    { "averageValues", context.Mapper.Map<IEnumerable<JsonAverageValue>>(region.AverageValues) }
                };

                collection.Add(new Feature(region.Geometry, attributes));
            }

            return collection;
        }
    }
}
