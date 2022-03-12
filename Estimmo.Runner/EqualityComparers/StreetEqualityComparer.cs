using Estimmo.Data.Entities;
using System;
using System.Collections.Generic;

namespace Estimmo.Runner.EqualityComparers
{
    public class StreetEqualityComparer : IEqualityComparer<Street>
    {
        public bool Equals(Street x, Street y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.Id == y.Id && x.Name == y.Name && x.TownId == y.TownId;
        }

        public int GetHashCode(Street obj)
        {
            return HashCode.Combine(obj.Id, obj.Name, obj.TownId);
        }
    }
}
