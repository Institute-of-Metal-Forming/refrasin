using System.Text.RegularExpressions;

namespace RefraSin.Coordinates.Helpers;

internal static class ParsingExtensions
{
    private static readonly Regex ParseRegex = new(
        @"^(?<type>[A-Za-z]\w*)?\s*[\(\[\<\{]?\s*(\w+\s*=\s*)?(?<value1>[\deE\+\-,\.]*\d(\s*[\w°]+)?)\s*[,;]?\s+(\w+\s*=\s*)?(?<value2>[\deE\+\-,\.]*\d)\s*[\)\]\>\}]?$"
    );

    /// <summary>
    ///     Umkehrung von
    ///     <see cref="ToString(IFormattable, IFormattable, string?, string?, string?, string?, string?, IFormatProvider?)" />.
    /// </summary>
    /// <param name="self">Eingabe string</param>
    /// <param name="typeName">Typname der zu erzeugenden Koordinaten (zur Prüfung, falls in <paramref name="self" /> vorhanden)</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static (string value1, string value2) ParseCoordinateString(
        this string self,
        string? typeName = null
    )
    {
        self = self.Trim();
        var match = ParseRegex.Match(self);
        if (match.Success)
        {
            var inputTypeName = match.Groups["type"].Value;
            if (
                string.IsNullOrEmpty(typeName)
                || string.IsNullOrEmpty(inputTypeName)
                || typeName == inputTypeName
            )
                return (match.Groups["value1"].Value, match.Groups["value2"].Value);

            throw new FormatException(
                $"String representation seems to encode an instance of {inputTypeName} instead of the requested {typeName}."
            );
        }

        throw new FormatException("Invalid format of input string.");
    }
}
