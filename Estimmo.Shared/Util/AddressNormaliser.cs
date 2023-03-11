using System.Text;
using System.Text.RegularExpressions;

namespace Estimmo.Shared.Util
{
    public class AddressNormaliser
    {
        private static readonly Dictionary<string, string> StreetSubtitutions = new()
        {
            { "Rte", "Route" },
            { "Imp", "Impasse" },
            { "Che", "Chemin" },
            { "Chem", "Chemin" },
            { "Av", "Avenue" },
            { "Pl", "Place" },
            { "All", "Allée" },
            { "Mte", "Montée" },
            { "Bd", "Boulevard" },
            { "Crs", "Cours" },
            { "Sq", "Square" },
            { "Grd", "Grand" }
        };

        public string NormaliseStreet(string street)
        {
            foreach (var (search, replace) in StreetSubtitutions)
            {
                if (!street.StartsWith(search, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                street = street.Replace(search, replace, StringComparison.OrdinalIgnoreCase);
                break;
            }

            // Normalise apostrophes
            street = street.Replace("’", "'");

            // Add missing apostrophes
            var missingApostropheMatch = Regex.Match(street, @"(?: |^)([ld]) ([\w]{2,})", RegexOptions.IgnoreCase);

            if (missingApostropheMatch.Success)
            {
                var article = missingApostropheMatch.Groups[1].Value;
                var word = missingApostropheMatch.Groups[2].Value;
                street = street.Replace(missingApostropheMatch.Value, $" {article}'{word}");
            }

            return ToTitleCaseString(street);
        }

        private static string ToTitleCaseString(string input)
        {
            var words = input
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var output = new StringBuilder();

            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i];
                var transformed = word;

                if (word.Substring(1, 1) == "'")
                {
                    if (word.Length > 5)
                    {
                        transformed = word[..2] + ToTitleCase(word[2..]);
                    }
                }
                else if (word.Length > 3 || i == 0)
                {
                    transformed = ToTitleCase(word);
                }

                output.Append(transformed);

                if (i < words.Length - 1)
                {
                    output.Append(' ');
                }
            }

            return output.ToString();
        }

        private static string ToTitleCase(string word)
        {
            return word[..1].ToUpper() + word[1..];
        }
    }
}
