namespace Estimmo.Data.Entities
{
    public class SectionValueStats : IValueStats
    {
        public string Id { get; set; }
        public PropertyType Type { get; set; }
        public string TownId { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
        public double? StandardDeviation { get; set; }

        public virtual Section Section { get; set; }
        public virtual Town Town { get; set; }
    }
}
