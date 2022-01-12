using AutoMapper;
using Estimmo.Api.Models.Features;
using NetTopologySuite.Features;
using System.Collections.Generic;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class RegionsTypeConverter : ITypeConverter<IEnumerable<RegionFeature>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<RegionFeature> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var feature in source)
            {
                var attributes = new AttributesTable
                {
                    { "id", feature.Region.Id },
                    { "name", feature.Region.Name },
                    { "averageValue", feature.AverageValue?.Value }
                };

                collection.Add(new Feature(feature.Region.Geometry, attributes));
            }

            return collection;
        }
    }
}
