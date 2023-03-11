using NetTopologySuite.Geometries;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Estimmo.Data.Entities
{
    public class PropertySale
    {
        private string _hash;

        public string Hash
        {
            get => _hash ??= ComputeHash();
            set => _hash = value;
        }

        public DateTime Date { get; set; }
        public string AddressId { get; set; }
        public PropertyType Type { get; set; }
        public int BuildingSurfaceArea { get; set; }
        public int LandSurfaceArea { get; set; }
        public short RoomCount { get; set; }
        public decimal Value { get; set; }
        public string SectionId { get; set; }
        public Point Coordinates { get; set; }

        public virtual Address Address { get; set; }
        public virtual Section Section { get; set; }

        private string ComputeHash()
        {
            var builder = new StringBuilder();

            builder.Append(Date.ToString("yyyy-MM-dd"));
            builder.Append('|');
            builder.Append(AddressId);
            builder.Append('|');
            builder.Append('|');
            builder.Append((int) Type);
            builder.Append('|');
            builder.Append(Value);

            var input = Encoding.UTF8.GetBytes(builder.ToString());
            return Convert.ToHexString(MD5.HashData(input));
        }
    }
}
