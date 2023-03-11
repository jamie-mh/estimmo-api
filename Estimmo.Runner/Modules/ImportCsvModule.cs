// Copyright (C) 2023 jmh
// SPDX-License-Identifier: GPL-3.0-only

using CsvHelper;
using CsvHelper.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public abstract class ImportCsvModule : IModule
    {
        private readonly ILogger _log = Log.ForContext<ImportCsvModule>();

        protected async Task ReadThenProcessFileAsync<T, TU>(string filePath, CsvConfiguration configuration, BatchListProcessor<T, TU> processor)
        {
            _log.Information("Reading {File}", filePath);
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, configuration);
            await processor.ProcessAsync(csv.GetRecordsAsync<T>());
        }

        protected async Task ReadThenProcessFileAsync<T>(string filePath, CsvConfiguration configuration, Func<IAsyncEnumerable<T>, Task> processor)
        {
            _log.Information("Reading {File}", filePath);
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, configuration);
            await processor(csv.GetRecordsAsync<T>());
        }

        public abstract Task RunAsync(Dictionary<string, string> args);
    }
}
