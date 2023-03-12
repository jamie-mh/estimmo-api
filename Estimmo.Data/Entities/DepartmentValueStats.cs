namespace Estimmo.Data.Entities
{
    public class DepartmentValueStats : IValueStats
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public string RegionId { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
        public double? StandardDeviation { get; set; }

        public virtual Department Department { get; set; }
        public virtual Region Region { get; set; }
    }
}
