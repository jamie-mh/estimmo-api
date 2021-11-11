using Estimmo.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class RefreshMaterialisedViews : IModule
    {
        private readonly EstimmoContext _context;

        public RefreshMaterialisedViews(EstimmoContext context)
        {
            _context = context;
        }

        public async Task RunAsync(string[] args)
        {
            await _context.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW region_avg_value");
            await _context.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW department_avg_value");
            await _context.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW town_avg_value");
            await _context.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW section_avg_value");
        }
    }
}
