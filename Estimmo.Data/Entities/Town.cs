// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Town
    {
        public Town()
        {
            Sections = new HashSet<Section>();
            SectionValueStats = new HashSet<SectionValueStats>();
            SectionValueStatsByYear = new HashSet<SectionValueStatsByYear>();
            ValueStats = new HashSet<TownValueStats>();
            ValueStatsByYear = new HashSet<TownValueStatsByYear>();
            Streets = new HashSet<Street>();
        }

        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string Name { get; set; }
        public string PostCode { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
        public virtual ICollection<SectionValueStats> SectionValueStats { get; set; }
        public virtual ICollection<SectionValueStatsByYear> SectionValueStatsByYear { get; set; }
        public virtual ICollection<TownValueStats> ValueStats { get; set; }
        public virtual ICollection<TownValueStatsByYear> ValueStatsByYear { get; set; }
        public virtual ICollection<Street> Streets { get; set; }
    }
}
