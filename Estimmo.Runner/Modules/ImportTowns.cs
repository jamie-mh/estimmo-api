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
                _context.Towns.AddRange(buffer);
                await _context.SaveChangesAsync();
                inserted += buffer.Count;
                _log.Information("Processed {Count} towns", inserted);
            }

            foreach (var feature in collection)
            {
                var id = feature.Attributes["id"].ToString();
                var departmentId = id[..2];
                var name = FormatName(feature.Attributes["nom"].ToString().ToLowerInvariant());

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

        private string FormatName(string name)
        {
            return _cultureInfo.TextInfo.ToTitleCase(name.ToLowerInvariant());
        }
    }
}
