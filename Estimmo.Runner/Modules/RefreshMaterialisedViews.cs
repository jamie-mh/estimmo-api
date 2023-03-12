using Estimmo.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class RefreshMaterialisedViews : IModule
    {
        private readonly EstimmoContext _context;
        private readonly ILogger _log = Log.ForContext<RefreshMaterialisedViews>();

        public RefreshMaterialisedViews(EstimmoContext context)
        {
            _context = context;
        }

        public async Task RunAsync(Dictionary<string, string> args)
        {
            var views = new[]
            {
                "france_value_stats", "france_value_stats_by_year", "region_value_stats", "region_value_stats_by_year",
                "department_value_stats", "department_value_stats_by_year", "town_value_stats", "town_value_stats_by_year",
                "section_value_stats", "section_value_stats_by_year", "place"
            };

            foreach (var view in views)
            {
                _log.Information("Refreshing view {View}", view);
                await _context.Database.ExecuteSqlRawAsync($"REFRESH MATERIALIZED VIEW {view}");
            }
        }
    }
}
