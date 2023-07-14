// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Department
    {
        public Department()
        {
            Towns = new HashSet<Town>();
            TownValueStats = new HashSet<TownValueStats>();
            TownValueStatsByYear = new HashSet<TownValueStatsByYear>();
            ValueStats = new HashSet<DepartmentValueStats>();
            ValueStatsByYear = new HashSet<DepartmentValueStatsByYear>();
        }

        public string Id { get; set; }
        public string RegionId { get; set; }
        public string Name { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Region Region { get; set; }
        public virtual ICollection<Town> Towns { get; set; }
        public virtual ICollection<TownValueStats> TownValueStats { get; set; }
        public virtual ICollection<TownValueStatsByYear> TownValueStatsByYear { get; set; }
        public virtual ICollection<DepartmentValueStats> ValueStats { get; set; }
        public virtual ICollection<DepartmentValueStatsByYear> ValueStatsByYear { get; set; }
    }
}
