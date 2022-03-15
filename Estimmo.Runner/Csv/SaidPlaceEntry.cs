using CsvHelper.Configuration.Attributes;

namespace Estimmo.Runner.Csv
{
    public class SaidPlaceEntry
    {
        [Name("id")] public string Id { get; set; }
        [Name("nom_lieu_dit")] public string Name { get; set; }
        [Name("code_postal")] public string PostCode { get; set; }
        [Name("code_insee")] public string InseeCode { get; set; }
        [Name("lon")] public double? Longitude { get; set; }
        [Name("lat")] public double? Latitude { get; set; }
    }
}
