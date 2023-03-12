namespace Estimmo.Data.Entities
{
    public class RegionAverageValueByYear : IAverageValueByYear
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public short Year { get; set; }
        public double Value { get; set; }
        public double? StandardDeviation { get; set; }

        public virtual Region Region { get; set; }
    }
}
