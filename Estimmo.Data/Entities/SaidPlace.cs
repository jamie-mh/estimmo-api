using NetTopologySuite.Geometries;

namespace Estimmo.Data.Entities
{
    public class SaidPlace
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TownId { get; set; }
        public string PostCode { get; set; }
        public Point Coordinates { get; set; }

        public virtual Town Town { get; set; }
    }
}
