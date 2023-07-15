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
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportAddresses : ImportCsvModule
    {
        private readonly ILogger _log = Log.ForContext<ImportAddresses>();
        private readonly EstimmoContext _context;
        private readonly TownIdsFixture _townIdsFixture;
        private readonly AddressNormaliser _addressNormaliser;
        private readonly CsvConfiguration _csvConfiguration;

        public ImportAddresses(EstimmoContext context, TownIdsFixture townIdsFixture,
            AddressNormaliser addressNormaliser)
        {
            _context = context;
            _townIdsFixture = townIdsFixture;
            _addressNormaliser = addressNormaliser;
            _csvConfiguration = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" };
        }

        public override async Task RunAsync(Dictionary<string, string> args)
        {
            await _townIdsFixture.LoadAsync();
            
            if (!args.ContainsKey("file"))
            {
                _log.Error("No CSV file specified");
                return;
            }

            var filePath = args["file"];
            await InsertStreetsAsync(filePath);
            await InsertAddressesAsync(filePath);
        }

        private static string GetStreetId(string inputId)
        {
            // Get only FANTOIR code from id, don't use dedicated column because its sometimes blank
            var idParts = inputId.Split('_');
            return idParts[0] + '_' + idParts[1];
        }

        private async Task InsertStreetsAsync(string filePath)
        {
            var townIds = await _townIdsFixture.GetValueAsync();
            
            var processor = new BatchProcessor<AddressEntry, Street>(entry =>
            {
                if (!townIds.Contains(entry.InseeCode))
                {
                    return null;
                }

                return new Street
                {
                    Id = GetStreetId(entry.Id),
                    TownId = entry.InseeCode,
                    Name = _addressNormaliser.NormaliseStreet(entry.StreetName)
                };
            }, async (buffer, processedCount) =>
            {
                await _context.Streets
                    .UpsertRange(buffer)
                    .On(s => new { s.Id })
                    .NoUpdate()
                    .RunAsync();

                _log.Information("Imported {Count} streets", processedCount);
            });

            _log.Information("Importing streets");
            await ReadThenProcessFileAsync(filePath, _csvConfiguration, processor);
        }

        private async Task InsertAddressesAsync(string filePath)
        {
            var townIds = await _townIdsFixture.GetValueAsync();
            
            var processor = new BatchProcessor<AddressEntry, Address>(entry =>
            {
                if (!townIds.Contains(entry.InseeCode))
                {
                    return null;
                }

                return new Address
                {
                    Id = entry.Id,
                    Number = entry.Number,
                    Suffix = string.IsNullOrEmpty(entry.Suffix) ? null : entry.Suffix,
                    PostCode = entry.PostCode,
                    StreetId = GetStreetId(entry.Id),
                    Coordinates = new Point(entry.Longitude, entry.Latitude)
                };
            }, async (buffer, processedCount) =>
            {
                await _context.Addresses
                    .UpsertRange(buffer)
                    .On(s => new { s.Id })
                    .NoUpdate()
                    .RunAsync();

                _log.Information("Imported {Count} addresses", processedCount);
            });

            _log.Information("Importing addresses");
            await ReadThenProcessFileAsync(filePath, _csvConfiguration, processor);
        }
    }
}
