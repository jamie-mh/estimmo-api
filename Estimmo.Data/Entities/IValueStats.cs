namespace Estimmo.Data.Entities
{
    public interface IValueStats
    {
        public PropertyType Type { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
        public double? StandardDeviation { get; set; }
    }
}
