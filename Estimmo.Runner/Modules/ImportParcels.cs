using Estimmo.Data;
using Estimmo.Data.Entities;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportParcels : IModule
    {
        private readonly ILogger _log = Log.ForContext<ImportParcels>();
        private readonly EstimmoContext _context;

        public ImportParcels(EstimmoContext context)
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
                var sectionCode = feature.Attributes["section"].ToString();

                if (!int.TryParse(feature.Attributes["numero"].ToString(), out var number))
                {
                    _log.Error("Failed to parse number");
                    continue;
                }

                var sectionId = $"{townId}{prefix}{sectionCode.PadLeft(2, '0')}";

                _context.Parcels.Add(new Parcel
                {
                    Id = id,
                    SectionId = sectionId,
                    TownId = townId,
                    Prefix = prefix,
                    Number = number,
                    SectionCode = sectionCode,
                    Geometry = feature.Geometry
                });

                _log.Information("Inserting parcel {Id}", id);

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
