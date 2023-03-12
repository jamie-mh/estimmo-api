namespace Estimmo.Data.Entities
{
    public interface IValueStatsByYear : IValueStats
    {
        public short Year { get; set; }
    }
}
