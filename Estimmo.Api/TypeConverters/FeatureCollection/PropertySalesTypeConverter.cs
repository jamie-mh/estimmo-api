using AutoMapper;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System.Collections.Generic;

namespace Estimmo.Api.TypeConverters.FeatureCollection
{
    public class PropertySalesTypeConverter : ITypeConverter<IEnumerable<PropertySale>, NetTopologySuite.Features.FeatureCollection>
    {
        public NetTopologySuite.Features.FeatureCollection Convert(IEnumerable<PropertySale> source, NetTopologySuite.Features.FeatureCollection destination, ResolutionContext context)
        {
            var collection = new NetTopologySuite.Features.FeatureCollection();

            foreach (var sale in source)
            {
                var attributes = new AttributesTable
                {
                    { "date", sale.Date },
                    { "streetNumber", sale.StreetNumber },
                    { "streetNumberSuffix", sale.StreetNumberSuffix },
                    { "streetName", sale.StreetName },
                    { "postCode", sale.PostCode },
                    { "type", sale.Type },
                    { "buildingSurfaceArea", sale.BuildingSurfaceArea },
                    { "landSurfaceArea", sale.LandSurfaceArea },
                    { "roomCount", sale.RoomCount },
                    { "value", sale.Value }
                };

                collection.Add(new Feature(sale.Coordinates, attributes));
            }

            return collection;
        }
    }
}
