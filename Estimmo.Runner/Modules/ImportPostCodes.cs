// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using CsvHelper.Configuration;
using Estimmo.Data;
using Estimmo.Runner.Csv;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
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

            var processor = new BatchProcessor<PostCodeEntry, FormattableString>(
                entry => $"UPDATE town SET post_code = '{entry.PostCode}' WHERE id = '{entry.InseeCode}'",
                async (buffer, count) =>
                {
                    var joined = String.Join(";", buffer);
                    await _context.Database.ExecuteSqlRawAsync(joined);
                    _log.Information("Imported {Count} postcodes", count);
                });

            await ReadThenProcessFileAsync(filePath,
                new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" }, processor);
        }
    }
}
