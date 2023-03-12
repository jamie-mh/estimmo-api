namespace Estimmo.Data.Entities
{
    public class TownAverageValueByYear : IAverageValueByYear
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public short Year { get; set; }
        public string DepartmentId { get; set; }
        public double Value { get; set; }
        public double? StandardDeviation { get; set; }

        public virtual Town Town { get; set; }
        public virtual Department Department { get; set; }
    }
}
