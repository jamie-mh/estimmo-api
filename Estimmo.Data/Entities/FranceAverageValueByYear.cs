namespace Estimmo.Data.Entities
{
    public class FranceAverageValueByYear : IAverageValueByYear
    {
        public PropertyType Type { get; set; }
        public short Year { get; set; }
        public double Value { get; set; }
    }
}
