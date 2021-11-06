using CsvHelper;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Estimmo.Runner.Csv;
using NetTopologySuite.Geometries;
using Serilog;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class ImportCsv : IModule
    {
        private const int BufferSize = 10000;

        private readonly ILogger _log = Log.ForContext<ImportCsv>();
        private readonly EstimmoContext _context;

        public ImportCsv(EstimmoContext context)
        {
            _context = context;
        }

        public async Task RunAsync(string[] args)
        {
            if (args.Length < 1)
            {
                _log.Error("No CSV file specified");
                return;
            }

            var filePath = args[0];
            _log.Information("Reading {File}", filePath);

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.CurrentCulture);
            var records = csv.GetRecordsAsync<PropertyMutation>();

            var buffer = new List<PropertySale>(BufferSize);
            var inserted = 0;

            async Task FlushBufferAsync()
            {
                _context.PropertySales.AddRange(buffer);
                await _context.SaveChangesAsync();
                _log.Information("Processed {Count} records", inserted);
            }

            await foreach (var record in records)
            {
                if (record.LocalType == null || record.MutationType != "Vente" ||
                    record.Value == null || record.BuildingSurfaceArea == null ||
                    record.RoomCount == null || record.Latitude == null || record.Longitude == null)
                {
                    continue;
                }

                PropertyType propertyType;

                switch (record.LocalType)
                {
                    case "Maison":
                        propertyType = PropertyType.House;
                        break;

                    case "Appartement":
                        propertyType = PropertyType.Apartment;
                        break;

                    default:
                        continue;
                }

                if (string.IsNullOrEmpty(record.StreetNumberSuffix))
                {
                    record.StreetNumberSuffix = null;
                }

                buffer.Add(new PropertySale
                {
                    Date = record.Date,
                    StreetNumber = record.StreetNumber,
                    StreetNumberSuffix = record.StreetNumberSuffix,
                    StreetName = record.StreetName,
                    PostCode = record.PostCode,
                    Type = propertyType,
                    BuildingSurfaceArea = record.BuildingSurfaceArea.Value,
                    LandSurfaceArea = record.LandSurfaceArea ?? 0,
                    RoomCount = record.RoomCount.Value,
                    Value = record.Value.Value,
                    Coordinates = new Point(record.Latitude.Value, record.Longitude.Value)
                });

                inserted++;

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

            _log.Information("Finished");
        }
    }
}
