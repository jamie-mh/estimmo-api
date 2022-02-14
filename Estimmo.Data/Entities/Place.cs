using NetTopologySuite.Geometries;

namespace Estimmo.Data.Entities
{
    public class Place
    {
        public PlaceType Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string SearchName { get; set; }
        public string PostCode { get; set; }
        public Geometry Geometry { get; set; }
    }
}
