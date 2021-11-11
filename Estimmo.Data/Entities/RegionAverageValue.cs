namespace Estimmo.Data.Entities
{
    public class RegionAverageValue : IAverageValue
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public double Value { get; set; }
    }
}
