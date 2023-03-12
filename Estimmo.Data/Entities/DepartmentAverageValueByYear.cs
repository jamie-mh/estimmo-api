namespace Estimmo.Data.Entities
{
    public class DepartmentAverageValueByYear : IAverageValueByYear
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public short Year { get; set; }
        public string RegionId { get; set; }
        public double Value { get; set; }
        public double? StandardDeviation { get; set; }

        public virtual Department Department { get; set; }
        public virtual Region Region { get; set; }
    }
}
