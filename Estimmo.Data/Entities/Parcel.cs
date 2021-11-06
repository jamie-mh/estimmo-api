using NetTopologySuite.Geometries;

namespace Estimmo.Data.Entities
{
    public class Parcel
    {
        public string Id { get; set; }
        public string TownId { get; set; }
        public string Prefix { get; set; }
        public string SectionCode { get; set; }
        public int Number { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Town Town { get; set; }
        public virtual Section Section { get; set; }
    }
}