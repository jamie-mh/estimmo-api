using Estimmo.Data;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportSections : IModule
    {
        private readonly ILogger _log = Log.ForContext<ImportSections>();
        private readonly EstimmoContext _context;

        public ImportSections(EstimmoContext context)
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

            _log.Information("Reading GeoJSON file {Name}", args[0]);

            var serialiser = GeoJsonSerializer.Create();
            using var streamReader = File.OpenText(args[0]);
            using var jsonReader = new JsonTextReader(streamReader);
            var collection = serialiser.Deserialize<FeatureCollection>(jsonReader);

            foreach (var feature in collection)
            {
                var id = feature.Attributes["id"].ToString();
                var townId = feature.Attributes["commune"].ToString();
                var prefix = feature.Attributes["prefixe"].ToString();
                var code = feature.Attributes["code"].ToString();

                _context.Sections.Add(new Section
                {
                    Id = id,
                    TownId = townId,
                    Prefix = prefix,
                    Code = code,
                    Geometry = feature.Geometry
                });

                _log.Information("Inserting section {Id}", id);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _log.Error(e, "An error occurred during insert");
                    _context.ChangeTracker.Clear();
                }
            }
        }
    }
}