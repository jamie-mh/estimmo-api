using CsvHelper.Configuration.Attributes;
using System;

namespace Estimmo.Runner.Csv
{
    public class PropertyMutation
    {
        [Name("date_mutation")]
        public DateTime Date { get; set; }

        [Name("nature_mutation")]
        public string MutationType { get; set; }

        [Name("valeur_fonciere")]
        public decimal? Value { get; set; }

        [Name("adresse_numero")]
        public short? StreetNumber { get; set; }

        [Name("adresse_suffixe")]
        public string StreetNumberSuffix { get; set; }

        [Name("adresse_nom_voie")]
        public string StreetName { get; set; }

        [Name("code_postal")]
        public string PostCode { get; set; }

        [Name("nom_commune")]
        public string Town { get; set; }

        [Name("type_local")]
        public string LocalType { get; set; }

        [Name("surface_reelle_bati")]
        public int? BuildingSurfaceArea { get; set; }

        [Name("nombre_pieces_principales")]
        public short? RoomCount { get; set; }

        [Name("surface_terrain")]
        public int? LandSurfaceArea { get; set; }

        [Name("latitude")]
        public double? Latitude { get; set; }

        [Name("longitude")]
        public double? Longitude { get; set; }
    }
}
