using AutoMapper;
using Estimmo.Api.Models.Features;
using NetTopologySuite.Features;
using System.Collections.Generic;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class SectionsTypeConverter : ITypeConverter<IEnumerable<SectionFeature>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<SectionFeature> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var feature in source)
            {
                var attributes = new AttributesTable
                {
                    { "id", feature.Section.Id },
                    { "averageValue", feature.AverageValue?.Value }
                };

                collection.Add(new Feature(feature.Section.Geometry, attributes));
            }

            return collection;
        }
    }
}
