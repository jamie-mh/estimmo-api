using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using Serilog;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportRegions : ImportFeatureCollectionModule
    {
        private readonly ILogger _log = Log.ForContext<ImportRegions>();
        private readonly EstimmoContext _context;

        public ImportRegions(EstimmoContext context)
        {
            _context = context;
        }

        protected override async Task ParseFeatureCollection(FeatureCollection collection)
        {
            foreach (var feature in collection)
            {
                var id = feature.Attributes["code"].ToString();
                var name = feature.Attributes["nom"].ToString();

                _log.Information("Importing region {Id} {Name}", id, name);
                var region = new Region { Id = id, Name = name, Geometry = feature.Geometry };

                await _context.Regions
                    .Upsert(region)
                    .On(s => new { s.Id })
                    .NoUpdate()
                    .RunAsync();
            }
        }
    }
}
