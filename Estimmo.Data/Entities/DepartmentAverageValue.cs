namespace Estimmo.Data.Entities
{
    public class DepartmentAverageValue : IAverageValue
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public string RegionId { get; set; }
        public double Value { get; set; }

        public virtual Region Region { get; set; }
    }
}
