using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System;
using System.Linq;

namespace Estimmo.Api
{
    public static class PointExt
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

        public static void CalculateBoundingBox(this FeatureCollection collection)
        {
            if (!collection.Any())
            {
                return;
            }

            var boundingBox = collection[0].Geometry.EnvelopeInternal;

            for (var i = 1; i < collection.Count; ++i)
            {
                boundingBox.ExpandToInclude(collection[i].Geometry.EnvelopeInternal);
            }

            collection.BoundingBox = boundingBox;
        }
    }
}
