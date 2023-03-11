using Estimmo.Shared.Extension;
using Estimmo.Test.Extension.ClassData;
using NetTopologySuite.Geometries;
using Xunit;

namespace Estimmo.Test.Extension
{
    public class PointExtensionTest
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
