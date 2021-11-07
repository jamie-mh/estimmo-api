﻿using CsvHelper;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Estimmo.Runner.Csv;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportPropertySales : IModule
    {
        private const int BufferSize = 10000;

        private static readonly Dictionary<string, string> NameSubtitutions = new()
        {
            { "Rte", "Route" },
            { "Imp", "Impasse" },
            { "Che", "Chemin" },
            { "Chem", "Chemin" },
            { "Av", "Avenue" },
            { "Pl", "Place" },
            { "All", "Allée" },
            { "Mte", "Montée" },
            { "Bd", "Boulevard" }
        };

        private readonly ILogger _log = Log.ForContext<ImportPropertySales>();
        private readonly EstimmoContext _context;
        private readonly CultureInfo _cultureInfo;

        public ImportPropertySales(EstimmoContext context)
        {
            _context = context;
            _cultureInfo = new CultureInfo("FR-fr");
        }

        public async Task RunAsync(string[] args)
        {
            if (args.Length < 1)
            {
                _log.Error("No CSV file specified");
                return;
            }

            var filePath = args[0];
            _log.Information("Reading {File}", filePath);

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.CurrentCulture);
            var mutations = csv.GetRecordsAsync<PropertyMutation>();
            await ParsePropertyMutationsAsync(mutations);
        }

        private async Task ParsePropertyMutationsAsync(IAsyncEnumerable<PropertyMutation> mutations)
        {
            _log.Information("Fetching parcel ids");
            var parcelIds = new HashSet<string>(await _context.Parcels.Select(p => p.Id).ToListAsync());

            var buffer = new List<PropertySale>(BufferSize);
            var inserted = 0;

            async Task FlushBufferAsync()
            {
                _context.PropertySales.AddRange(buffer);
                await _context.SaveChangesAsync();
                inserted += buffer.Count;
                _log.Information("Processed {Count} mutations", inserted);
            }

            await foreach (var mutation in mutations)
            {
                if (mutation.LocalType == null || mutation.MutationType != "Vente" ||
                    mutation.Value == null || mutation.BuildingSurfaceArea == null ||
                    mutation.RoomCount == null || mutation.Latitude == null || mutation.Longitude == null ||
                    !parcelIds.Contains(mutation.ParcelId))
                {
                    continue;
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

                    default: continue;
                }

                if (string.IsNullOrEmpty(mutation.StreetNumberSuffix))
                {
                    mutation.StreetNumberSuffix = null;
                }

                buffer.Add(new PropertySale
                {
                    Date = mutation.Date,
                    StreetNumber = mutation.StreetNumber,
                    StreetNumberSuffix = mutation.StreetNumberSuffix,
                    StreetName = FormatStreetName(mutation.StreetName),
                    PostCode = mutation.PostCode,
                    Type = propertyType,
                    BuildingSurfaceArea = mutation.BuildingSurfaceArea.Value,
                    LandSurfaceArea = mutation.LandSurfaceArea ?? 0,
                    RoomCount = mutation.RoomCount.Value,
                    Value = mutation.Value.Value,
                    ParcelId = mutation.ParcelId,
                    Coordinates = new Point(mutation.Longitude.Value, mutation.Latitude.Value)
                });

                if (buffer.Count % BufferSize == 0)
                {
                    await FlushBufferAsync();
                    buffer.Clear();
                }
            }

            if (buffer.Any())
            {
                await FlushBufferAsync();
            }
        }

        private string FormatStreetName(string street)
        {
            foreach (var (search, replace) in NameSubtitutions)
            {
                if (!street.StartsWith(search, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                street = street.Replace(search, replace, StringComparison.OrdinalIgnoreCase);
                break;
            }

            return _cultureInfo.TextInfo.ToTitleCase(street.ToLowerInvariant());
        }
    }
}