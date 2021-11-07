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

            foreach (var section in source)
            {
                var attributes = new AttributesTable
                {
                    { "id", section.Id }
                };

                collection.Add(new Feature(section.Geometry, attributes));
            }

            return collection;
        }
    }
}
