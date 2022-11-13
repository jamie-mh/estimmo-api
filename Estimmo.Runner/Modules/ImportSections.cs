using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Overlay.Snap;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportSections : ImportFeatureCollectionModule
    {
        private const int BufferSize = 10000;

        private readonly ILogger _log = Log.ForContext<ImportSections>();
        private readonly EstimmoContext _context;

        public ImportSections(EstimmoContext context)
        {
            _context = context;
        }

        protected override async Task ParseFeatureCollection(FeatureCollection collection)
        {
            var townIds = new HashSet<string>(await _context.Towns.Select(t => t.Id).ToListAsync());

            var buffer = new List<Section>(BufferSize);
            var inserted = 0;

            async Task FlushBufferAsync()
            {
                await _context.Sections
                    .UpsertRange(buffer)
                    .On(s => new { s.Id })
                    .WhenMatched((current, next) => new Section
                    {
                        Code = current.Code,
                        Prefix = current.Prefix,
                        TownId = current.TownId,
                        Geometry = next.Geometry
                    })
                    .RunAsync();

                inserted += buffer.Count;
                _log.Information("Processed {Count} sections", inserted);
            }

            foreach (var feature in collection)
            {
                var townId = feature.Attributes["commune"].ToString();

                if (!townIds.Contains(townId))
                {
                    continue;
                }

                var town = await _context.Towns.FirstOrDefaultAsync(t => t.Id == townId);

                if (town == null)
                {
                    continue;
                }

                Geometry geometry;

                try
                {
                    geometry = town.Geometry.Intersection(feature.Geometry);
                }
                catch (TopologyException e)
                {
                    _log.Warning(e, "Failed to calculate intersection");
                    geometry = feature.Geometry;
                }

                var id = feature.Attributes["id"].ToString();
                var prefix = feature.Attributes["prefixe"].ToString();
                var code = feature.Attributes["code"].ToString();

                buffer.Add(new Section
                {
                    Id = id,
                    TownId = townId,
                    Prefix = prefix,
                    Code = code,
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
