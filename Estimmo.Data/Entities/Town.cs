using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Town
    {
        public Town()
        {
            Sections = new HashSet<Section>();
            Parcels = new HashSet<Parcel>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Geometry Geometry { get; set; }

        public virtual ICollection<Section> Sections { get; set; }
        public virtual ICollection<Parcel> Parcels { get; set; }
    }
}
