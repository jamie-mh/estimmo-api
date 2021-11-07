using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportParcels : ImportFeatureCollectionModule
    {
        private const int BufferSize = 10000;

        private readonly ILogger _log = Log.ForContext<ImportParcels>();
        private readonly EstimmoContext _context;

        public ImportParcels(EstimmoContext context)
        {
            _context = context;
        }

        protected override async Task ParseFeatureCollection(FeatureCollection collection)
        {
            var sectionIds = new HashSet<string>(await _context.Sections.Select(s => s.Id).ToListAsync());

            var buffer = new List<Parcel>(BufferSize);
            var inserted = 0;

            async Task FlushBufferAsync()
            {
                _context.Parcels.AddRange(buffer);
                await _context.SaveChangesAsync();
                inserted += buffer.Count;
                _log.Information("Processed {Count} parcels", inserted);
            }

            foreach (var feature in collection)
            {
                var id = feature.Attributes["id"].ToString();
                var townId = feature.Attributes["commune"].ToString();
                var prefix = feature.Attributes["prefixe"].ToString();
                var sectionCode = feature.Attributes["section"].ToString();

                if (!int.TryParse(feature.Attributes["numero"].ToString(), out var number))
                {
                    _log.Error("Failed to parse number : {@Number}", feature.Attributes["numero"]);
                    continue;
                }

                var sectionId = $"{townId}{prefix}{sectionCode.PadLeft(2, '0')}";

                if (!sectionIds.Contains(sectionId))
                {
                    continue;
                }

                buffer.Add(new Parcel
                {
                    Id = id,
                    SectionId = sectionId,
                    TownId = townId,
                    Prefix = prefix,
                    Number = number,
                    SectionCode = sectionCode,
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
