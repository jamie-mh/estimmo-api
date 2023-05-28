using NetTopologySuite.Features;
using System.Linq;

namespace Estimmo.Api.Extension
{
    public static class FeaturesExtension
    {
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
