namespace Estimmo.Data.Entities
{
    public class RegionValueStats : IValueStats
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
        public double? StandardDeviation { get; set; }

        public virtual Region Region { get; set; }
    }
}
