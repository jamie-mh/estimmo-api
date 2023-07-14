using Estimmo.Shared.Extension;
using Xunit;

namespace Estimmo.Test.Extension
{
    public class StringExtensionTest
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData("test", "test")]
        [InlineData("éééé", "eeee")]
        [InlineData("èèèè", "eeee")]
        [InlineData("ëëëë", "eeee")]
        [InlineData("êêêê", "eeee")]
        [InlineData("àààà", "aaaa")]
        [InlineData("ïïïï", "iiii")]
        [InlineData("çççç", "cccc")]
        public void UnaccentTest(string input, string expected)
        {
            Assert.Equal(expected, input.Unaccent());
            Assert.Equal(expected?.ToUpper(), input?.ToUpper().Unaccent());
        }
    }
}
