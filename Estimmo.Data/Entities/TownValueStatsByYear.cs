namespace Estimmo.Data.Entities
{
    public class TownValueStatsByYear : IValueStatsByYear
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public short Year { get; set; }
        public string DepartmentId { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
        public double? StandardDeviation { get; set; }

        public virtual Town Town { get; set; }
        public virtual Department Department { get; set; }
    }
}
