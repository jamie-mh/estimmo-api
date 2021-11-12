namespace Estimmo.Data.Entities
{
    public class TownAverageValue : IAverageValue
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public string DepartmentId { get; set; }
        public double Value { get; set; }

        public Department Department { get; set; }
    }
}