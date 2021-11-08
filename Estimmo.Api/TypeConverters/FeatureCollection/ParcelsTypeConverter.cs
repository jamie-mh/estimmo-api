using AutoMapper;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System.Collections.Generic;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class ParcelsTypeConverter : ITypeConverter<IEnumerable<Parcel>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<Parcel> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var parcel in source)
            {
                var attributes = new AttributesTable
                {
                    { "id", parcel.Id }
                };

                collection.Add(new Feature(parcel.Geometry, attributes));
            }

            return collection;
        }
    }
}
