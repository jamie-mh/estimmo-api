using CsvHelper;
using CsvHelper.Configuration;
using Estimmo.Data;
using Estimmo.Runner.Csv;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportPostCodes : IModule
    {
        private readonly ILogger _log = Log.ForContext<ImportPostCodes>();
        private readonly EstimmoContext _context;

        public ImportPostCodes(EstimmoContext context)
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
            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" };
            using var csv = new CsvReader(reader, config);
            var entries = csv.GetRecordsAsync<PostCodeEntry>();

            await foreach (var entry in entries)
            {
                _log.Information("Updating {Town}", entry.TownName);
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE town SET post_code = {0} WHERE id = {1}", entry.PostCode, entry.InseeCode);
            }
        }
    }
}
