// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using NetTopologySuite.Geometries;

namespace Estimmo.Data.Entities
{
    public class Place
    {
        public PlaceType Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string SearchName { get; set; }
        public string PostCode { get; set; }
        public PlaceType? ParentType { get; set; }
        public string ParentId { get; set; }
        public bool IsSearchable { get; set; }
        public Geometry Geometry { get; set; }

        public virtual Place Parent { get; set; }
    }
}
