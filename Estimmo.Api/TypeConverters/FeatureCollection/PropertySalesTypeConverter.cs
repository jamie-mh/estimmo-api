using AutoMapper;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using System;
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
                var id = HashCode.Combine(sale.Date, sale.AddressId, sale.Value);

                var attributes = new AttributesTable
                {
                    { "featureId", Math.Abs(id) },
                    { "date", sale.Date },
                    { "streetNumber", sale.Address.Number },
                    { "streetNumberSuffix", sale.Address.Suffix },
                    { "streetName", sale.Address.Street.Name },
                    { "postCode", sale.Address.PostCode },
                    { "type", sale.Type },
                    { "buildingSurfaceArea", sale.BuildingSurfaceArea },
                    { "landSurfaceArea", sale.LandSurfaceArea },
                    { "roomCount", sale.RoomCount },
                    { "value", sale.Value }
                };

                collection.CalculateBoundingBox();
                collection.Add(new Feature(sale.Coordinates, attributes));
            }

            return collection;
        }
    }
}
