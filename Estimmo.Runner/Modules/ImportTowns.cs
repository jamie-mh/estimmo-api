// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Serilog;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportTowns : ImportFeatureCollectionModule
    {
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
            var departments = await _context.Departments.ToListAsync();
            
            var processor = new BatchProcessor<IFeature, Town>(feature =>
            {
                var id = feature.Attributes.Exists("id")
                    ? feature.Attributes["id"].ToString()
                    : feature.Attributes["code"].ToString();

                var departmentId = id.StartsWith("97") ? id[..3] : id[..2];
                var department = departments.FirstOrDefault(d => d.Id == departmentId);

                if (department == null)
                {
                    return null;
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

                return new Town
                {
                    Id = id,
                    DepartmentId = departmentId,
                    Name = name,
                    Geometry = geometry
                };
            }, async (buffer, processedCount) =>
            {
                await _context.Towns
                    .UpsertRange(buffer)
                    .On(t => new { t.Id })
                    .NoUpdate()
                    .RunAsync();

                _log.Information("Imported {Count} towns", processedCount);
            });

            await processor.ProcessAsync(collection);
        }
    }
}
