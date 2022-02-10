using Estimmo.Data;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using Serilog;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportTowns : ImportFeatureCollectionModule
    {
        private const int BufferSize = 10000;

        private readonly ILogger _log = Log.ForContext<ImportTowns>();
        private readonly EstimmoContext _context;

        protected override double SimplificationDistanceTolerance => 0.005d;

        public ImportTowns(EstimmoContext context)
        {
            _context = context;
        }

        protected override async Task ParseFeatureCollection(FeatureCollection collection)
        {
            var buffer = new List<Town>(BufferSize);
            var inserted = 0;

            async Task FlushBufferAsync()
            {
                _context.Towns.AddRange(buffer);
                await _context.SaveChangesAsync();
                inserted += buffer.Count;
                _log.Information("Processed {Count} towns", inserted);
            }

            foreach (var feature in collection)
            {
                var id = feature.Attributes["code"].ToString();
                var departmentId = id[..2];
                var name = feature.Attributes["nom"].ToString();

                buffer.Add(new Town
                {
                    Id = id,
                    DepartmentId = departmentId,
                    Name = name,
                    Geometry = feature.Geometry
                });

                if (buffer.Count % BufferSize == 0)
                {
                    await FlushBufferAsync();
                    buffer.Clear();
                }
            }

            if (buffer.Any())
            {
                await FlushBufferAsync();
            }
        }
    }
}
