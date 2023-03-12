using Estimmo.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class CalculateStreetCoordinates : IModule
    {
        private readonly EstimmoContext _context;

        public CalculateStreetCoordinates(EstimmoContext context)
        {
            _context = context;
        }

        public async Task RunAsync(Dictionary<string, string> args)
        {
            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE street s SET coordinates = (SELECT ST_Centroid(ST_Union(a.coordinates)) FROM address a
                WHERE a.street_id = s.id) WHERE s.coordinates IS NULL");
        }
    }
}
