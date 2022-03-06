using NetTopologySuite.Geometries;
using System.Collections;
using System.Collections.Generic;

namespace Estimmo.Test
{
    public class GreatCircleDistanceTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Point(-4.48625, 48.39023), new Point(-4.49569, 48.40686),
                1976.0
            };
            yield return new object[]
            {
                new Point(-4.48631, 48.39029), new Point(-3.82825, 48.57765),
                52786.0
            };
            yield return new object[]
            {
                new Point(-4.494693823109529, 48.40713384429712), new Point(-4.497044178079073, 48.40618089139661),
                203.32
            };
            yield return new object[]
            {
                new Point(-4.495402107832264, 48.40682173359243), new Point(-4.485174798369439, 48.409104515221834),
                796.34
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
