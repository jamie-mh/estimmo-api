// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using CsvHelper;
using CsvHelper.Configuration;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public abstract class ImportCsvModule : IModule
    {
        private readonly ILogger _log = Log.ForContext<ImportCsvModule>();

        protected async Task ReadThenProcessFileAsync<T, TU>(string filePath, CsvConfiguration configuration, BatchProcessor<T, TU> processor)
        {
            _log.Information("Reading {File}", filePath);
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, configuration);
            await processor.ProcessAsync(csv.GetRecordsAsync<T>());
        }

        public abstract Task RunAsync(Dictionary<string, string> args);
    }
}
