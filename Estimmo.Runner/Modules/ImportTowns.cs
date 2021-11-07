using Estimmo.Data;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportTowns : IModule
    {
        private const int BufferSize = 10000;

        private readonly ILogger _log = Log.ForContext<ImportTowns>();
        private readonly EstimmoContext _context;

        public ImportTowns(EstimmoContext context)
        {
            _context = context;
        }

        public async Task RunAsync(string[] args)
        {
            if (args.Length < 1)
            {
                _log.Error("No GeoJSON file specified");
                return;
            }

            var serialiser = GeoJsonSerializer.Create();

            _log.Information("Reading GeoJSON file {Name}", args[0]);
            using var streamReader = File.OpenText(args[0]);
            using var jsonReader = new JsonTextReader(streamReader);
            var collection = serialiser.Deserialize<FeatureCollection>(jsonReader);

            var culture = new CultureInfo("FR-fr");

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
                var name = culture.TextInfo.ToTitleCase(feature.Attributes["nom"].ToString().ToLowerInvariant());

                buffer.Add(new Town { Id = id, Name = name, Geometry = feature.Geometry });

                if (buffer.Count % BufferSize == 0)
                {
                    await FlushBufferAsync();
                    buffer.Clear();
                }
            }

            await FlushBufferAsync();
        }
    }
}
