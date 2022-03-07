using Estimmo.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class MigrateDatabase : IModule
    {
        private EstimmoContext _context;
        private ILogger _log = Log.ForContext<MigrateDatabase>();

        public MigrateDatabase(EstimmoContext context)
        {
            _context = context;
        }

        public async Task RunAsync(List<string> args)
        {
            var pending = (await _context.Database.GetPendingMigrationsAsync()).ToList();

            if (!pending.Any())
            {
                _log.Information("Database is already up to date");
                return;
            }

            _log.Information("Applying migrations {Migrations}", pending);
            await _context.Database.MigrateAsync();
        }
    }
}
