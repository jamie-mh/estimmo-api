using AutoMapper;
using Estimmo.Api.Entities.Json;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
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
                    { "id", town.Id },
                    { "name", town.Name },
                    { "averageValues", town.AverageValues.ToDictionary(r => (int) r.Type, r => r.Value) }
                };

                collection.Add(new Feature(town.Geometry, attributes));
            }

            return collection;
        }
    }
}
