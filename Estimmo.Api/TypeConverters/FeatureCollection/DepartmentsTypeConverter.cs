using AutoMapper;
using Estimmo.Api.Entities.Json;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System.Collections.Generic;
using System.Linq;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class DepartmentsTypeConverter : ITypeConverter<IEnumerable<Department>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<Department> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var department in source)
            {
                var attributes = new AttributesTable
                {
                    { "id", department.Id },
                    { "regionId", department.RegionId },
                    { "name", department.Name },
                    { "averageValues", department.AverageValues.ToDictionary(r => (int) r.Type, r => r.Value) }
                };

                collection.Add(new Feature(department.Geometry, attributes));
            }

            return collection;
        }
    }
}
