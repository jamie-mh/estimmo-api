using Estimmo.Shared;
using NetTopologySuite.Geometries;
using Xunit;

namespace Estimmo.Test
{
    public class PointExtTest
    {
        [Theory]
        [ClassData(typeof(GreatCircleDistanceTestData))]
        public void GreatCircleDistanceTest(Point a, Point b, double expectedDistance)
        {
            var distance = a.GreatCircleDistance(b);
            Assert.Equal(distance, expectedDistance, 0);
        }
    }
}
