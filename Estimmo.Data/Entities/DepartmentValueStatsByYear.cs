namespace Estimmo.Data.Entities
{
    public class DepartmentValueStatsByYear : IValueStatsByYear
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public short Year { get; set; }
        public string RegionId { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
        public double? StandardDeviation { get; set; }

        public virtual Department Department { get; set; }
        public virtual Region Region { get; set; }
    }
}
