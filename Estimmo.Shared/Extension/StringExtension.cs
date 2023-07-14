// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text;
using System.Text.RegularExpressions;

namespace Estimmo.Shared.Extension
{
    public static partial class StringExtension
    {
        [GeneratedRegex("\\p{Mn}")]
        private static partial Regex NormaliseRegex();
        
        public static string Unaccent(this string input)
        {
            if (input == null)
            {
                return null;
            }

            var normalised = input.Normalize(NormalizationForm.FormD);
            return NormaliseRegex().Replace(normalised, "");
        }
    }
}
