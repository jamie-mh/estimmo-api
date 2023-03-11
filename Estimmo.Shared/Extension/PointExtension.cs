using NetTopologySuite.Geometries;

namespace Estimmo.Shared.Extension
{
    public static class PointExtension
    {
        private const double EarthRadius = 6371e3;

        private static double DegreesToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }

        // https://en.wikipedia.org/wiki/Great-circle_distance#Formulae
        public static double GreatCircleDistance(this Point a, Point b)
        {
            var lonARad = DegreesToRadians(a.X);
            var latARad = DegreesToRadians(a.Y);
            var lonBRad = DegreesToRadians(b.X);
            var latBRad = DegreesToRadians(b.Y);

            var deltaLon = Math.Abs(lonARad - lonBRad);
            var centralAngle =
                Math.Acos(Math.Sin(latARad) * Math.Sin(latBRad) +
                          Math.Cos(latARad) * Math.Cos(latBRad) * Math.Cos(deltaLon));

            return centralAngle * EarthRadius;
        }
    }
}
