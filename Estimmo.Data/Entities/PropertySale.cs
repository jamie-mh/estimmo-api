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
        public short? StreetNumber { get; set; }
        public string StreetNumberSuffix { get; set; }
        public string StreetName { get; set; }
        public string PostCode { get; set; }
        public PropertyType Type { get; set; }
        public int BuildingSurfaceArea { get; set; }
        public int LandSurfaceArea { get; set; }
        public short RoomCount { get; set; }
        public decimal Value { get; set; }
        public string SectionId { get; set; }
        public Point Coordinates { get; set; }

        public virtual Section Section { get; set; }

        private string ComputeHash()
        {
            var builder = new StringBuilder();

            builder.Append(Date.ToString("yyyy-MM-dd"));
            builder.Append('|');

            if (StreetNumber != null)
            {
                builder.Append(StreetNumber);
                builder.Append('|');
            }

            if (StreetNumberSuffix != null)
            {
                builder.Append(StreetNumberSuffix);
                builder.Append('|');
            }

            builder.Append(StreetName);
            builder.Append('|');
            builder.Append(PostCode);
            builder.Append('|');
            builder.Append((int) Type);
            builder.Append('|');
            builder.Append(Value);

            var input = Encoding.UTF8.GetBytes(builder.ToString());

            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(input);

            return Convert.ToHexString(hash);
        }
    }
}
