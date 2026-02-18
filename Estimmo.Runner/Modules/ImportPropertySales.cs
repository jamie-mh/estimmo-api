// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using CsvHelper.Configuration;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Estimmo.Runner.Csv;
using Estimmo.Runner.Fixtures;
using Estimmo.Shared.Utility;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportPropertySales : ImportCsvModule
    {
        private readonly ILogger _log = Log.ForContext<ImportPropertySales>();
        private readonly EstimmoContext _context;
        private readonly SectionIdsFixture _sectionIdsFixture;
        private readonly AddressNormaliser _addressNormaliser;

        public ImportPropertySales(EstimmoContext context, SectionIdsFixture sectionIdsFixture,
            AddressNormaliser addressNormaliser)
        {
            _context = context;
            _sectionIdsFixture = sectionIdsFixture;
            _addressNormaliser = addressNormaliser;
        }

        public override async Task RunAsync(Dictionary<string, string> args)
        {
            if (!args.ContainsKey("file"))
            {
                _log.Error("No CSV file specified");
                return;
            }

            var sectionIds = await _sectionIdsFixture.LoadAndGetValueAsync();
            var filePath = args["file"];

            var processor = new BatchProcessor<PropertyMutation, PropertySale>(mutation =>
            {
                if (mutation.LocalType == null || mutation.MutationType != "Vente" ||
                    mutation.Value == null || mutation.BuildingSurfaceArea == null ||
                    mutation.RoomCount == null || mutation.Latitude == null || mutation.Longitude == null ||
                    mutation.ParcelId == null)
                {
                    return null;
                }

                PropertyType propertyType;

                switch (mutation.LocalType)
                {
                    case "Maison":
                        propertyType = PropertyType.House;
                        break;

                    case "Appartement":
                        propertyType = PropertyType.Apartment;
                        break;

                    default: return null;
                }

                var sectionId = mutation.ParcelId[..^4];

                if (!sectionIds.Contains(sectionId))
                {
                    return null;
                }

                if (string.IsNullOrEmpty(mutation.StreetNumberSuffix))
                {
                    mutation.StreetNumberSuffix = null;
                }

                return WithComputedHash(new PropertySale
                {
                    Date = mutation.Date,
                    StreetNumber = mutation.StreetNumber,
                    StreetNumberSuffix = mutation.StreetNumberSuffix,
                    StreetName = _addressNormaliser.NormaliseStreet(mutation.StreetName),
                    PostCode = mutation.PostCode,
                    Type = propertyType,
                    BuildingSurfaceArea = mutation.BuildingSurfaceArea.Value,
                    LandSurfaceArea = mutation.LandSurfaceArea ?? 0,
                    RoomCount = mutation.RoomCount.Value,
                    Value = mutation.Value.Value,
                    SectionId = sectionId,
                    Coordinates = new Point(mutation.Longitude.Value, mutation.Latitude.Value)
                });
            }, async (buffer, processedCount) =>
            {
                await _context.PropertySales
                    .UpsertRange(buffer)
                    .On(t => new { t.Hash })
                    .NoUpdate()
                    .RunAsync();

                _log.Information("Imported {Count} mutations", processedCount);
            });

            await ReadThenProcessFileAsync(filePath, new CsvConfiguration(CultureInfo.CurrentCulture), processor);
        }

        private static PropertySale WithComputedHash(PropertySale sale)
        {
            var builder = new StringBuilder();

            builder.Append(sale.Date.ToString("yyyy-MM-dd"));
            builder.Append('|');

            if (sale.StreetNumber != null)
            {
                builder.Append(sale.StreetNumber);
                builder.Append('|');
            }

            if (sale.StreetNumberSuffix != null)
            {
                builder.Append(sale.StreetNumberSuffix);
                builder.Append('|');
            }

            builder.Append(sale.StreetName);
            builder.Append('|');
            builder.Append(sale.PostCode);
            builder.Append('|');
            builder.Append((int) sale.Type);
            builder.Append('|');
            builder.Append(sale.Value);

            var input = Encoding.UTF8.GetBytes(builder.ToString());
            sale.Hash = Convert.ToHexString(MD5.HashData(input));

            return sale;
        }
    }
}
