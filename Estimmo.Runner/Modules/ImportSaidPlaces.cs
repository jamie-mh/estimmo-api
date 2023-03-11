using CsvHelper.Configuration;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Estimmo.Runner.Csv;
using Estimmo.Shared.Util;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Serilog;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportSaidPlaces : ImportCsvModule
    {
        private readonly ILogger _log = Log.ForContext<ImportSaidPlaces>();
        private readonly EstimmoContext _context;
        private readonly AddressNormaliser _addressNormaliser;

        public ImportSaidPlaces(EstimmoContext context, AddressNormaliser addressNormaliser)
        {
            _context = context;
            _addressNormaliser = addressNormaliser;
        }

        public override async Task RunAsync(Dictionary<string, string> args)
        {
            if (!args.ContainsKey("file"))
            {
                _log.Error("No CSV file specified");
                return;
            }

            _log.Information("Populating town ID lookup");
            var townIds = new HashSet<string>(await _context.Towns.Select(t => t.Id).ToListAsync());

            var processor = new BatchListProcessor<SaidPlaceEntry, SaidPlace>(entry =>
            {
                if (!townIds.Contains(entry.InseeCode) || entry.Latitude == null || entry.Longitude == null)
                {
                    return null;
                }

                return new SaidPlace
                {
                    Id = entry.Id,
                    Name = _addressNormaliser.NormaliseStreet(entry.Name),
                    PostCode = entry.PostCode,
                    TownId = entry.InseeCode,
                    Coordinates = new Point(entry.Longitude.Value, entry.Latitude.Value)
                };
            }, async (buffer, processedCount) =>
            {
                await _context.SaidPlaces
                    .UpsertRange(buffer)
                    .On(t => new { t.Id })
                    .NoUpdate()
                    .RunAsync();

                _log.Information("Imported {Count} said places", processedCount);
            });

            var filePath = args["file"];
            await ReadThenProcessFileAsync(filePath, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" }, processor);
        }
    }
}
