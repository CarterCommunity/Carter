// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Globalization;
using System.Linq;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Extensions class for strings to handle special characters.
    /// </summary>
    public static class SpecialCharacterStringExtensions
    {
        // Plain style strings cannot start with indicators. 
        // http://www.yaml.org/spec/1.2/spec.html#indicator//
        private static readonly char[] _yamlIndicators =
        {
            '-',
            '?',
            ':',
            ',',
            '{',
            '}',
            '[',
            ']',
            '&',
            '*',
            '#',
            '?',
            '|',
            '-',
            '>',
            '!',
            '%',
            '@',
            '`',
            '\'',
            '"',
        };

        // Plain style strings cannot contain these character combinations.
        // http://www.yaml.org/spec/1.2/spec.html#style/flow/plain
        private static readonly string[] _yamlPlainStringForbiddenCombinations =
        {
            ": ",
            " #",

            // These are technically forbidden only inside flow collections, but
            // for the sake of simplicity, we will never allow them in our generated plain string.
            "[",
            "]",
            "{",
            "}",
            ","
        };

        // Plain style strings cannot end with these characters.
        // http://www.yaml.org/spec/1.2/spec.html#style/flow/plain
        private static readonly string[] _yamlPlainStringForbiddenTerminals =
        {
            ":"
        };

        // Double-quoted strings are needed for these non-printable control characters.
        // http://www.yaml.org/spec/1.2/spec.html#style/flow/double-quoted
        private static readonly char[] _yamlControlCharacters =
        {
            '\0',
            '\x01',
            '\x02',
            '\x03',
            '\x04',
            '\x05',
            '\x06',
            '\a',
            '\b',
            '\t',
            '\n',
            '\v',
            '\f',
            '\r',
            '\x0e',
            '\x0f',
            '\x10',
            '\x11',
            '\x12',
            '\x13',
            '\x14',
            '\x15',
            '\x16',
            '\x17',
            '\x18',
            '\x19',
            '\x1a',
            '\x1b',
            '\x1c',
            '\x1d',
            '\x1e',
            '\x1f'
        };

        /// <summary>
        /// Escapes all special characters and put the string in quotes if necessary to
        /// get a YAML-compatible string.
        /// </summary>
        internal static string GetYamlCompatibleString(this string input)
        {
            // If string is an empty string, wrap it in quote to ensure it is not recognized as null.
            if (input == "")
            {
                return "''";
            }

            // If string is the word null, wrap it in quote to ensure it is not recognized as empty scalar null.
            if (input == "null")
            {
                return "'null'";
            }

            // If string is the letter ~, wrap it in quote to ensure it is not recognized as empty scalar null.
            if (input == "~")
            {
                return "'~'";
            }

            // If string includes a control character, wrapping in double quote is required.
            if (input.Any(c => _yamlControlCharacters.Contains(c)))
            {
                // Replace the backslash first, so that the new backslashes created by other Replaces are not duplicated.
                input = input.Replace("\\", "\\\\");

                // Escape the double quotes.
                input = input.Replace("\"", "\\\"");

                // Escape all the control characters.
                input = input.Replace("\0", "\\0");
                input = input.Replace("\x01", "\\x01");
                input = input.Replace("\x02", "\\x02");
                input = input.Replace("\x03", "\\x03");
                input = input.Replace("\x04", "\\x04");
                input = input.Replace("\x05", "\\x05");
                input = input.Replace("\x06", "\\x06");
                input = input.Replace("\a", "\\a");
                input = input.Replace("\b", "\\b");
                input = input.Replace("\t", "\\t");
                input = input.Replace("\n", "\\n");
                input = input.Replace("\v", "\\v");
                input = input.Replace("\f", "\\f");
                input = input.Replace("\r", "\\r");
                input = input.Replace("\x0e", "\\x0e");
                input = input.Replace("\x0f", "\\x0f");
                input = input.Replace("\x10", "\\x10");
                input = input.Replace("\x11", "\\x11");
                input = input.Replace("\x12", "\\x12");
                input = input.Replace("\x13", "\\x13");
                input = input.Replace("\x14", "\\x14");
                input = input.Replace("\x15", "\\x15");
                input = input.Replace("\x16", "\\x16");
                input = input.Replace("\x17", "\\x17");
                input = input.Replace("\x18", "\\x18");
                input = input.Replace("\x19", "\\x19");
                input = input.Replace("\x1a", "\\x1a");
                input = input.Replace("\x1b", "\\x1b");
                input = input.Replace("\x1c", "\\x1c");
                input = input.Replace("\x1d", "\\x1d");
                input = input.Replace("\x1e", "\\x1e");
                input = input.Replace("\x1f", "\\x1f");

                return $"\"{input}\"";
            }

            // If string 
            // 1) includes a character forbidden in plain string,
            // 2) starts with an indicator, OR 
            // 3) has trailing/leading white spaces,
            // wrap the string in single quote.
            // http://www.yaml.org/spec/1.2/spec.html#style/flow/plain
            if (_yamlPlainStringForbiddenCombinations.Any(fc => input.Contains(fc)) ||
                _yamlIndicators.Any(i => input.StartsWith(i.ToString())) ||
                _yamlPlainStringForbiddenTerminals.Any(i => input.EndsWith(i.ToString())) ||
                input.Trim() != input)
            {
                // Escape single quotes with two single quotes.
                input = input.Replace("'", "''");

                return $"'{input}'";
            }

            // If string can be mistaken as a number, a boolean, or a timestamp,
            // wrap it in quote to indicate that this is indeed a string, not a number, a boolean, or a timestamp
            if (decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var _) ||
                bool.TryParse(input, out var _) ||
                DateTime.TryParse(input, out var _))
            {
                return $"'{input}'";
            }

            return input;
        }

        /// <summary>
        /// Handles control characters and backslashes and adds double quotes
        /// to get JSON-compatible string.
        /// </summary>
        internal static string GetJsonCompatibleString(this string value)
        {
            if (value == null)
            {
                return "null";
            }

            // Show the control characters as strings
            // http://json.org/

            // Replace the backslash first, so that the new backslashes created by other Replaces are not duplicated.
            value = value.Replace("\\", "\\\\");

            value = value.Replace("\b", "\\b");
            value = value.Replace("\f", "\\f");
            value = value.Replace("\n", "\\n");
            value = value.Replace("\r", "\\r");
            value = value.Replace("\t", "\\t");
            value = value.Replace("\"", "\\\"");

            return $"\"{value}\"";
        }
    }
}