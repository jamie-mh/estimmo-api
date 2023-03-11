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
                "france_avg_value", "france_avg_value_by_year", "region_avg_value", "region_avg_value_by_year",
                "department_avg_value", "department_avg_value_by_year", "town_avg_value", "town_avg_value_by_year",
                "section_avg_value", "section_avg_value_by_year", "place"
            };

            foreach (var view in views)
            {
                _log.Information("Refreshing view {View}", view);
                await _context.Database.ExecuteSqlRawAsync($"REFRESH MATERIALIZED VIEW {view}");
            }
        }
    }
}
