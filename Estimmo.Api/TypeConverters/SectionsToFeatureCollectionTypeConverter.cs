using AutoMapper;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System.Collections.Generic;

namespace Estimmo.Api.TypeConverters
{
    public class SectionsToFeatureCollectionTypeConverter : ITypeConverter<IEnumerable<Section>, FeatureCollection>
    {
        public FeatureCollection Convert(IEnumerable<Section> source, FeatureCollection destination, ResolutionContext context)
        {
            var collection = new FeatureCollection();

            foreach (var town in source)
            {
                var attributes = new AttributesTable
                {
                    { "id", town.Id }
                };

                collection.Add(new Feature(town.Geometry, attributes));
            }

            return collection;
        }
    }
}
