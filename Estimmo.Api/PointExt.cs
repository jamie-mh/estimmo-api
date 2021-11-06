using NetTopologySuite.Geometries;
using System;

namespace Estimmo.Api
{
    public static class PointExt
    {
        private const double EarthRadius = 6371e3;

        private static double ToRadians(double x)
        {
            return x * Math.PI / 180;
        }

        public static double GreatCircleDistance(this Point a, Point b)
        {
            var sinLatA = Math.Sin(ToRadians(a.X));
            var sinLatB = Math.Sin(ToRadians(b.X));
            var cosLatA = Math.Cos(ToRadians(a.X));
            var cosLatB= Math.Cos(ToRadians(b.X));
            var cosLon = Math.Cos(ToRadians(a.Y) - ToRadians(b.Y));

            var relDistance = Math.Acos(sinLatA * sinLatB + cosLatA * cosLatB * cosLon);
            return relDistance * EarthRadius;
        }
    }
}
