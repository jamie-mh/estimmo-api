using CsvHelper.Configuration.Attributes;

namespace Estimmo.Runner.Csv
{
    public class AddressEntry
    {
        [Name("id")] public string Id { get; set; }
        [Name("numero")] public int? Number { get; set; }
        [Name("rep")] public string Suffix { get; set; }
        [Name("nom_voie")] public string StreetName { get; set; }
        [Name("code_postal")] public string PostCode { get; set; }
        [Name("code_insee")] public string InseeCode { get; set; }
        [Name("lon")] public double Longitude { get; set; }
        [Name("lat")] public double Latitude { get; set; }
    }
}
