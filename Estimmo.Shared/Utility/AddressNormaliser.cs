using System.Text;
using System.Text.RegularExpressions;

namespace Estimmo.Shared.Utility
{
    public partial class AddressNormaliser
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
            { "Grd", "Grand" },
            { "Rpe", "Rampe" }
        };

        private static readonly List<string> LowercaseWords = new(){ "de", "la", "du", "des", "les", "en" };

        private static readonly List<string> UppercaseWords = new(){ "zi" };

        [GeneratedRegex(@"(?: |^)([ld]) ([\w]{2,})", RegexOptions.IgnoreCase)]
        private static partial Regex MissingApostropheRegex();

        public string NormaliseStreet(string input)
        {
            // Normalise apostrophes
            input = input.Replace("’", "'");

            // Add missing apostrophes
            var missingApostropheMatch = MissingApostropheRegex().Match(input);

            if (missingApostropheMatch.Success)
            {
                var article = missingApostropheMatch.Groups[1].Value;
                var word = missingApostropheMatch.Groups[2].Value;
                input = input.Replace(missingApostropheMatch.Value, $" {article}'{word}");
            }

            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var (search, replace) in StreetSubtitutions)
            {
                for (var i = 0; i < words.Length; i++)
                {
                    var word = words[i];

                    if (String.Equals(word, search, StringComparison.OrdinalIgnoreCase))
                    {
                        words[i] = replace;
                    }
                }
            }

            return ToTitleCase(words);
        }

        private static string ToTitleCase(IReadOnlyList<string> words)
        {
            var output = new StringBuilder();

            for (var i = 0; i < words.Count; i++)
            {
                var word = words[i].ToLower();
                var transformed = word;

                if (i > 0 && word.Length > 3 && word.Substring(1, 1) == "'")
                {
                    var afterApostrophe = word[2..];

                    if (!LowercaseWords.Contains(afterApostrophe))
                    {
                        afterApostrophe = UppercaseWords.Contains(afterApostrophe)
                            ? afterApostrophe.ToUpper()
                            : ToTitleCase(afterApostrophe);

                        transformed = word[..2] + afterApostrophe;
                    }
                }
                else if (i == 0 || !LowercaseWords.Contains(word))
                {
                    transformed = UppercaseWords.Contains(word)
                        ? word.ToUpper()
                        : ToTitleCase(word);
                }

                output.Append(transformed);

                if (i < words.Count - 1)
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
