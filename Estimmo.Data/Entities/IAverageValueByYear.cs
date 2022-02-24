namespace Estimmo.Data.Entities
{
    public interface IAverageValueByYear : IAverageValue
    {
        public short Year { get; set; }
    }
}
