using NetTopologySuite.Features;
using NetTopologySuite.IO;
using NetTopologySuite.Simplify;
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
        protected abstract double SimplificationDistanceTolerance { get; }

        public async Task RunAsync(List<string> args)
        {
            if (!args.Any())
            {
                _log.Error("No GeoJSON file specified");
                return;
            }

            var serialiser = GeoJsonSerializer.Create();
            using var streamReader = File.OpenText(args[0]);
            using var jsonReader = new JsonTextReader(streamReader);

            FeatureCollection collection = null;

            _log.Information("Reading GeoJSON file {Name}", args[0]);
            await Task.Run(delegate
            {
                collection = serialiser.Deserialize<FeatureCollection>(jsonReader);
            });

            _log.Information("Simplifying geometry");
            foreach (var feature in collection)
            {
                var simplifier = new VWSimplifier(feature.Geometry) { DistanceTolerance = SimplificationDistanceTolerance };
                feature.Geometry = simplifier.GetResultGeometry();
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
