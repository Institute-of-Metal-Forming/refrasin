using System.Text.RegularExpressions;
using RefraSin.Coordinates.Helpers;

namespace RefraSin.Coordinates;

/// <summary>
///     Abstract base class for coordinates without information about their system.
/// </summary>
public abstract class Coordinates : ICoordinates
{
    private static readonly Regex ParseRegex = new(
        @"^(?<type>[A-Za-z]\w*)?\s*[\(\[\<\{]?\s*(\w+\s*=\s*)?(?<value1>[\deE\+\-,\.]*\d(\s*[\w°]+)?)\s*[,;]?\s+(\w+\s*=\s*)?(?<value2>[\deE\+\-,\.]*\d)\s*[\)\]\>\}]?$"
    );

    /// <inheritdoc />
    public abstract void RotateBy(Angle angle);

    /// <inheritdoc />
    public abstract double[] ToArray();

    /// <summary>
    ///     String representation of this object. Formats with distinct format strings.
    /// </summary>
    /// <param name="value1">Wert der ersten Koordinate</param>
    /// <param name="value2">Wert der zweiten Koordinate</param>
    /// <param name="coordinatesFormat">Format der Koordinatendarstellung</param>
    /// <param name="format1">Format der ersten Koordinate</param>
    /// <param name="format2">Format der zweiten Koordinate</param>
    /// <param name="name1">Name of <paramref name="value1" /> to show with format 'V'</param>
    /// <param name="name2">Name of <paramref name="value1" /> to show with format 'V'</param>
    /// <param name="formatProvider">IFormatProvider</param>
    /// <remarks>
    ///     <paramref name="coordinatesFormat" /> valid control characters:
    ///     <list>
    ///         <listheader>
    ///             <term> control character </term> <description> effect </description>
    ///         </listheader>
    ///         <item>
    ///             <term> ( ) { } [ ] &lt; &gt; </term> <description> add opening and closing brackets </description>
    ///         </item>
    ///         <item>
    ///             <term> ; , </term> <description> use as delimiter between the dimensions </description>
    ///         </item>
    ///         <item>
    ///             <term> V </term> <description> be verbose, always add "X = " etc. </description>
    ///         </item>
    ///         <item>
    ///             <term> N </term> <description> add the term "Strain" in front </description>
    ///         </item>
    ///     </list>
    ///     It doesn't matter in which order the control characters are supplied.
    /// </remarks>
    /// <exception cref="FormatException">invalid format</exception>
    protected string ToString(IFormattable value1, IFormattable value2, string? coordinatesFormat, string? format1, string? format2,
        string? name1, string? name2, IFormatProvider? formatProvider)
    {
        coordinatesFormat ??= string.Empty;

        var strainFormatChars = coordinatesFormat.ToCharArray();

        var openingBracket = strainFormatChars switch
        {
            _ when strainFormatChars.Contains('[') => '[',
            _ when strainFormatChars.Contains('(') => '(',
            _ when strainFormatChars.Contains('{') => '{',
            _ when strainFormatChars.Contains('<') => '<',
            _                                      => ' '
        };

        var closingBracket = strainFormatChars switch
        {
            _ when strainFormatChars.Contains(']') => ']',
            _ when strainFormatChars.Contains(')') => ')',
            _ when strainFormatChars.Contains('}') => '}',
            _ when strainFormatChars.Contains('>') => '>',
            _                                      => ' '
        };

        var delimiter = strainFormatChars switch
        {
            _ when strainFormatChars.Contains(',') => ", ",
            _ when strainFormatChars.Contains(';') => "; ",
            _                                      => " "
        };

        var valueString = strainFormatChars.Contains('V')
            ? $"{name1} = {value1.ToString(format1, formatProvider)}{delimiter}{name2} = {value2.ToString(format2, formatProvider)}"
            : $"{value1.ToString(format1, formatProvider)}{delimiter}{value2.ToString(format2, formatProvider)}";

        var prefix = strainFormatChars.Contains('N') ? GetType().Name : string.Empty;

        return $"{prefix}{openingBracket}{valueString}{closingBracket}".Trim();
    }

    /// <summary>
    ///     Umkehrung von
    ///     <see cref="ToString(IFormattable, IFormattable, string?, string?, string?, string?, string?, IFormatProvider?)" />.
    /// </summary>
    /// <param name="s">Eingabe string</param>
    /// <param name="typeName">Typname der zu erzeugenden Koordinaten (zur Prüfung, falls in <paramref name="s" /> vorhanden)</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    protected static (string value1, string value2) Parse(string s, string? typeName = null)
    {
        s = s.Trim();
        var match = ParseRegex.Match(s);
        if (match.Success)
        {
            var inputTypeName = match.Groups["type"].Value;
            if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(inputTypeName) || typeName == inputTypeName)
                return (match.Groups["value1"].Value, match.Groups["value2"].Value);

            throw new FormatException(
                $"String representation seems to encode an instance of {inputTypeName} instead of the requested {typeName}.");
        }

        throw new FormatException("Invalid format of input string.");
    }

    /// <inheritdoc />
    public abstract string ToString(string? format, IFormatProvider? formatProvider);
}