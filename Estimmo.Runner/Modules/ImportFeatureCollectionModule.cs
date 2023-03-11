using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public abstract class ImportFeatureCollectionModule : IModule
    {
        private readonly ILogger _log = Log.ForContext<ImportFeatureCollectionModule>();

        public async Task RunAsync(Dictionary<string, string> args)
        {
            if (!args.ContainsKey("file"))
            {
                _log.Error("No GeoJSON file specified");
                return;
            }

            var filePath = args["file"];
            using var streamReader = File.OpenText(filePath);

            await using var jsonReader = new JsonTextReader(streamReader);
            var serialiser = GeoJsonSerializer.Create();

            FeatureCollection collection = null;

            _log.Information("Reading GeoJSON file {Name}", filePath);
            await Task.Run(delegate
            {
                collection = serialiser.Deserialize<FeatureCollection>(jsonReader);
            });

            _log.Information("Removing invalid features");
            foreach (var feature in collection.Where(f => f.Geometry == null).ToList())
            {
                collection.Remove(feature);
            }

            _log.Information("Calculating bounding boxes");
            foreach (var feature in collection)
            {
                feature.BoundingBox ??= feature.Geometry.EnvelopeInternal;
            }

            _log.Information("Parsing collection");
            await ParseFeatureCollection(collection);
        }

        protected abstract Task ParseFeatureCollection(FeatureCollection collection);
    }
}
