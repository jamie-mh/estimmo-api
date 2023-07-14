// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportSections : ImportFeatureCollectionModule
    {
        private readonly ILogger _log = Log.ForContext<ImportSections>();
        private readonly EstimmoContext _context;

        public ImportSections(EstimmoContext context)
        {
            _context = context;
        }

        protected override async Task ParseFeatureCollection(FeatureCollection collection)
        {
            _log.Information("Populating town ID lookup");
            var townIds = new HashSet<string>(await _context.Towns.Select(t => t.Id).ToListAsync());

            var processor = new BatchProcessor<IFeature, Section>(async feature =>
            {
                var townId = feature.Attributes["commune"].ToString();

                if (!townIds.Contains(townId))
                {
                    return null;
                }

                var town = await _context.Towns.FirstOrDefaultAsync(t => t.Id == townId);

                if (town == null)
                {
                    return null;
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

                return new Section
                {
                    Id = id,
                    TownId = townId,
                    Prefix = prefix,
                    Code = code,
                    Geometry = geometry
                };
            }, async (buffer, processedCount) =>
            {
                await _context.Sections
                    .UpsertRange(buffer)
                    .On(t => new { t.Id })
                    .NoUpdate()
                    .RunAsync();

                _log.Information("Imported {Count} sections", processedCount);
            });

            await processor.ProcessAsync(collection);
        }
    }
}
