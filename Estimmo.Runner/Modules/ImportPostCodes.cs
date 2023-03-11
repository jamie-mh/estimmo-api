using CsvHelper.Configuration;
using Estimmo.Data;
using Estimmo.Runner.Csv;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportPostCodes : ImportCsvModule
    {
        private readonly ILogger _log = Log.ForContext<ImportPostCodes>();
        private readonly EstimmoContext _context;

        public ImportPostCodes(EstimmoContext context)
        {
            _context = context;
        }

        public override async Task RunAsync(Dictionary<string, string> args)
        {
            if (!args.ContainsKey("file"))
            {
                _log.Error("No CSV file specified");
                return;
            }

            var filePath = args["file"];

            await ReadThenProcessFileAsync<SaidPlaceEntry>(filePath,
                new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" },
                async entries =>
                {
                    var count = 0;

                    await foreach (var entry in entries)
                    {
                        await _context.Database.ExecuteSqlRawAsync(
                            "UPDATE town SET post_code = {0} WHERE id = {1}", entry.PostCode, entry.InseeCode);

                        if (count > 0 && count % 1000 == 0)
                        {
                            _log.Information("Processed {Count} postcodes", count);
                        }

                        count++;
                    }
                });
        }
    }
}
