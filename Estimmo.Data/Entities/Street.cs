// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Estimmo.Data.Entities
{
    public class Street
    {
        public Street()
        {
            Addresses = new HashSet<Address>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string TownId { get; set; }
        public Point Coordinates { get; set; }

        public virtual Town Town { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}
