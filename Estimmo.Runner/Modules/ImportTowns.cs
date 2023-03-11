using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
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
        private readonly CultureInfo _cultureInfo;
        private readonly EstimmoContext _context;

        public ImportTowns(EstimmoContext context)
        {
            _context = context;
            _cultureInfo = new CultureInfo("FR-fr");
        }

        protected override async Task ParseFeatureCollection(FeatureCollection collection)
        {
            var buffer = new List<Town>(BufferSize);
            var inserted = 0;

            async Task FlushBufferAsync()
            {
                await _context.Towns
                    .UpsertRange(buffer)
                    .On(t => new { t.Id })
                    .WhenMatched((current, next) => new Town
                    {
                        DepartmentId = current.DepartmentId,
                        PostCode = current.PostCode,
                        Name = next.Name,
                        Geometry = next.Geometry
                    })
                    .RunAsync();

                inserted += buffer.Count;
                _log.Information("Processed {Count} towns", inserted);
            }

            foreach (var feature in collection)
            {
                var id = feature.Attributes.Exists("id")
                    ? feature.Attributes["id"].ToString()
                    : feature.Attributes["code"].ToString();

                var departmentId = id.StartsWith("97") ? id[..3] : id[..2];
                var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == departmentId);

                if (department == null)
                {
                    continue;
                }

                Geometry geometry;

                try
                {
                    geometry = department.Geometry.Intersection(feature.Geometry);
                }
                catch (TopologyException e)
                {
                    _log.Warning(e, "Failed to calculate intersection");
                    geometry = feature.Geometry;
                }

                var name = _cultureInfo.TextInfo.ToTitleCase(feature.Attributes["nom"].ToString().ToLowerInvariant());

                buffer.Add(new Town
                {
                    Id = id,
                    DepartmentId = departmentId,
                    Name = name,
                    Geometry = geometry
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
