using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using Serilog;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportDepartments : ImportFeatureCollectionModule
    {
        private readonly ILogger _log = Log.ForContext<ImportDepartments>();
        private readonly EstimmoContext _context;

        protected override double SimplificationDistanceTolerance => 0.02d;

        public ImportDepartments(EstimmoContext context)
        {
            _context = context;
        }

        protected override async Task ParseFeatureCollection(FeatureCollection collection)
        {
            foreach (var feature in collection)
            {
                var id = feature.Attributes["code"].ToString();
                var name = feature.Attributes["nom"].ToString();

                var centroid = feature.Geometry.Centroid;
                var enclosingRegion = await _context.Regions.FirstOrDefaultAsync(r => r.Geometry.Covers(centroid));

                if (enclosingRegion == null)
                {
                    continue;
                }

                _log.Information("Importing department {Id} {Name} in {Region}", id, name, enclosingRegion.Name);

                _context.Departments.Add(new Department
                {
                    Id = id, Name = name, RegionId = enclosingRegion.Id, Geometry = feature.Geometry
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
