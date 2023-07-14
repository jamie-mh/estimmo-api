// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Section
    {
        public Section()
        {
            PropertySales = new HashSet<PropertySale>();
            ValueStats = new HashSet<SectionValueStats>();
            ValueStatsByYear = new HashSet<SectionValueStatsByYear>();
        }

        public string Id { get; set; }
        public string TownId { get; set; }
        public string Prefix { get; set; }
        public string Code { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Town Town { get; set; }
        public virtual ICollection<PropertySale> PropertySales { get; set; }
        public virtual ICollection<SectionValueStats> ValueStats { get; set; }
        public virtual ICollection<SectionValueStatsByYear> ValueStatsByYear { get; set; }
    }
}
