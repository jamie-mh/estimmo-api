using System.Text;
using System.Text.RegularExpressions;

namespace Estimmo.Shared.Extension
{
    public static class StringExtension
    {
        public static string Unaccent(this string input)
        {
            if (input == null)
            {
                return null;
            }

            var normalised = input.Normalize(NormalizationForm.FormD);
            return Regex.Replace(normalised, @"\p{Mn}", "");
        }
    }
}
