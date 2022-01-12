using AutoMapper;
using Estimmo.Api.Models.Features;
using NetTopologySuite.Features;
using System.Collections.Generic;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class DepartmentsTypeConverter : ITypeConverter<IEnumerable<DepartmentFeature>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<DepartmentFeature> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var feature in source)
            {
                var attributes = new AttributesTable
                {
                    { "id", feature.Department.Id },
                    { "regionId", feature.Department.RegionId },
                    { "name", feature.Department.Name },
                    { "averageValue", feature.AverageValue?.Value }
                };

                collection.Add(new Feature(feature.Department.Geometry, attributes));
            }

            return collection;
        }
    }
}
