using CsvHelper;
using CsvHelper.Configuration;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Estimmo.Runner.Csv;
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
    public class ImportSaidPlaces : IModule
    {
        private readonly ILogger _log = Log.ForContext<ImportSaidPlaces>();
        private readonly EstimmoContext _context;

        public ImportSaidPlaces(EstimmoContext context)
        {
            _context = context;
        }

        public async Task RunAsync(List<string> args)
        {
            if (!args.Any())
            {
                _log.Error("No CSV file specified");
                return;
            }

            var filePath = args[0];
            _log.Information("Reading {File}", filePath);

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" });
            var entries = csv.GetRecordsAsync<SaidPlaceEntry>();

            _log.Information("Fetching town ids");
            var townIds = new HashSet<string>(await _context.Towns.Select(t => t.Id).ToListAsync());

            var saidPlaces = new List<SaidPlace>();

            _log.Information("Processing entries");

            await foreach (var entry in entries)
            {
                if (!townIds.Contains(entry.InseeCode) || entry.Latitude == null || entry.Longitude == null)
                {
                    continue;
                }

                saidPlaces.Add(new SaidPlace
                {
                    Id = entry.Id,
                    Name = entry.Name.Replace("’", "'"),
                    PostCode = entry.PostCode,
                    TownId = entry.InseeCode,
                    Coordinates = new Point(entry.Longitude.Value, entry.Latitude.Value)
                });
            }

            _context.SaidPlaces.AddRange(saidPlaces);
            await _context.SaveChangesAsync();

            _log.Information("Imported {Count} said places", saidPlaces.Count);
        }
    }
}
