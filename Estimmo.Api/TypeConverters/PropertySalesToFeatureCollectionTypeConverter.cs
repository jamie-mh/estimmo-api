using AutoMapper;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System.Collections.Generic;

namespace Estimmo.Api.TypeConverters
{
    public class PropertySalesToFeatureCollectionTypeConverter : ITypeConverter<IEnumerable<PropertySale>, FeatureCollection>
    {
        public FeatureCollection Convert(IEnumerable<PropertySale> source, FeatureCollection destination, ResolutionContext context)
        {
            var collection = new FeatureCollection();

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
