using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Section
    {
        public Section()
        {
            Parcels = new HashSet<Parcel>();
        }

        public string Id { get; set; }
        public string TownId { get; set; }
        public string Prefix { get; set; }
        public string Code { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Town Town { get; set; }
        public ICollection<Parcel> Parcels { get; set; }
    }
}