using CsvHelper;
using CsvHelper.Configuration;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Estimmo.Runner.Csv;
using Estimmo.Runner.EqualityComparers;
using Estimmo.Shared.Util;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Serilog;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportAddresses : IModule
    {
        private readonly ILogger _log = Log.ForContext<ImportAddresses>();
        private readonly EstimmoContext _context;
        private readonly AddressNormaliser _addressNormaliser;

        public ImportAddresses(EstimmoContext context, AddressNormaliser addressNormaliser)
        {
            _context = context;
            _addressNormaliser = addressNormaliser;
        }

        public async Task RunAsync(Dictionary<string, string> args)
        {
            if (!args.ContainsKey("file"))
            {
                _log.Error("No CSV file specified");
                return;
            }

            var filePath = args["file"];
            _log.Information("Reading {File}", filePath);

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" });
            var entries = csv.GetRecordsAsync<AddressEntry>();

            _log.Information("Fetching town ids");
            var townIds = new HashSet<string>(await _context.Towns.Select(t => t.Id).ToListAsync());

            var streets = new HashSet<Street>(new StreetEqualityComparer());
            var addresses = new List<Address>();

            _log.Information("Processing entries");

            await foreach (var entry in entries)
            {
                if (!townIds.Contains(entry.InseeCode))
                {
                    continue;
                }

                // Get only FANTOIR code from id, don't use dedicated column because its sometimes blank
                var idParts = entry.Id.Split('_');
                var streetId = idParts[0] + '_' + idParts[1];

                var street = new Street
                {
                    Id = streetId,
                    TownId = entry.InseeCode,
                    Name = _addressNormaliser.NormaliseStreet(entry.StreetName)
                };

                streets.Add(street);

                addresses.Add(new Address
                {
                    Id = entry.Id,
                    Number = entry.Number,
                    Suffix = string.IsNullOrEmpty(entry.Suffix) ? null : entry.Suffix,
                    PostCode = entry.PostCode,
                    StreetId = streetId,
                    Coordinates = new Point(entry.Longitude, entry.Latitude)
                });
            }

            _context.Streets.AddRange(streets);
            _context.Addresses.AddRange(addresses);
            await _context.SaveChangesAsync();

            _log.Information("Imported {Streets} streets and {Addresses} addresses", streets.Count, addresses.Count);
        }
    }
}
