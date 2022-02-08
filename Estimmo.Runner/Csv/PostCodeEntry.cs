using CsvHelper.Configuration.Attributes;

namespace Estimmo.Runner.Csv
{
    public class PostCodeEntry
    {
        [Name("Code_commune_INSEE")] public string InseeCode { get; set; }
        [Name("Nom_commune")] public string TownName { get; set; }
        [Name("Code_postal")] public string PostCode { get; set; }
    }
}
