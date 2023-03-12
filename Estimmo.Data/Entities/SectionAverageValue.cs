namespace Estimmo.Data.Entities
{
    public class SectionAverageValue : IAverageValue
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public string TownId { get; set; }
        public double Value { get; set; }
        public double? StandardDeviation { get; set; }

        public virtual Section Section { get; set; }
        public virtual Town Town { get; set; }
    }
}
