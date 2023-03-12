namespace Estimmo.Data.Entities
{
    public interface IAverageValue
    {
        public PropertyType Type { get; set; }
        public double Value { get; set; }
        public double? StandardDeviation { get; set; }
    }
}
