namespace Estimmo.Data.Entities
{
    public class FranceValueStatsByYear : IValueStatsByYear
    {
        public PropertyType Type { get; set; }
        public short Year { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
        public double? StandardDeviation { get; set; }
    }
}
