namespace Estimmo.Data.Entities
{
    public class FranceValueStats : IValueStats
    {
        public PropertyType Type { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
        public double? StandardDeviation { get; set; }
    }
}
