// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ExecuteSqlFile : IModule
    {
        private readonly ILogger _log = Log.ForContext<ExecuteSqlFile>();
        private readonly EstimmoContext _context;

        public ExecuteSqlFile(EstimmoContext context)
        {
            _context = context;
        }

        public async Task RunAsync(Dictionary<string, string> args)
        {
            if (!args.ContainsKey("file"))
            {
                _log.Error("No SQL file specified");
                return;
            }

            var file = args["file"];
            _log.Information("Read file {File}", file);

            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Join(currentDir, "sql", file);
            var sql = await File.ReadAllTextAsync(path);

            _log.Information("Executing SQL\n{Sql}", sql);
            var rows = await _context.Database.ExecuteSqlRawAsync(sql);
            _log.Information("{Rows} rows affected", rows);
        }
    }
}
