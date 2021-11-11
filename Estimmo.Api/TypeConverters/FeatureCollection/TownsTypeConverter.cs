using AutoMapper;
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

            foreach (var town in source)
            {
                var attributes = new AttributesTable
                {
                    { "id", town.Id },
                    { "name", town.Name }
                };

                collection.Add(new Feature(town.Point, attributes));
            }

            return collection;
        }
    }
}
