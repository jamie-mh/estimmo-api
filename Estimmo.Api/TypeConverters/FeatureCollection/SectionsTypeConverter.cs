using AutoMapper;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System.Collections.Generic;

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
                    { "id", section.Id }
                };

                collection.Add(new Feature(section.Geometry, attributes));
            }

            return collection;
        }
    }
}
