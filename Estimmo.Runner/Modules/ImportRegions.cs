using Estimmo.Data;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using Serilog;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportRegions : ImportFeatureCollectionModule
    {
        private readonly ILogger _log = Log.ForContext<ImportRegions>();
        private readonly EstimmoContext _context;

        protected override double SimplificationDistanceTolerance => 0.04d;

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
                _context.Regions.Add(new Region { Id = id, Name = name, Geometry = feature.Geometry });
            }

            await _context.SaveChangesAsync();
        }
    }
}
