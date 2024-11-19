using RefraSin.Coordinates.Cartesian;
using RefraSin.Coordinates.Polar;

namespace RefraSin.Coordinates.Helpers;

internal static class FormattingExtensions
{
    public static string FormatCoordinates<TCoordinates>(
        this TCoordinates self,
        IFormattable value1,
        IFormattable value2,
        string? coordinatesFormat,
        string? format1,
        string? format2,
        string? name1,
        string? name2,
        IFormatProvider? formatProvider
    )
        where TCoordinates : ICoordinates
    {
        coordinatesFormat ??= string.Empty;

        var strainFormatChars = coordinatesFormat.ToCharArray();

        var openingBracket = strainFormatChars switch
        {
            _ when strainFormatChars.Contains('[') => '[',
            _ when strainFormatChars.Contains('(') => '(',
            _ when strainFormatChars.Contains('{') => '{',
            _ when strainFormatChars.Contains('<') => '<',
            _ => ' ',
        };

        var closingBracket = strainFormatChars switch
        {
            _ when strainFormatChars.Contains(']') => ']',
            _ when strainFormatChars.Contains(')') => ')',
            _ when strainFormatChars.Contains('}') => '}',
            _ when strainFormatChars.Contains('>') => '>',
            _ => ' ',
        };

        var delimiter = strainFormatChars switch
        {
            _ when strainFormatChars.Contains(',') => ", ",
            _ when strainFormatChars.Contains(';') => "; ",
            _ => " ",
        };

        var valueString = strainFormatChars.Contains('V')
            ? $"{name1} = {value1.ToString(format1, formatProvider)}{delimiter}{name2} = {value2.ToString(format2, formatProvider)}"
            : $"{value1.ToString(format1, formatProvider)}{delimiter}{value2.ToString(format2, formatProvider)}";

        var prefix = strainFormatChars.Contains('N') ? self.GetType().Name : string.Empty;

        return $"{prefix}{openingBracket}{valueString}{closingBracket}".Trim();
    }

    public static string FormatCartesianCoordinates<TCoordinates>(
        this TCoordinates self,
        string? coordinatesFormat,
        string? numberFormat,
        IFormatProvider? formatProvider
    )
        where TCoordinates : ICartesianCoordinates =>
        FormatCoordinates(
            self,
            self.X,
            self.Y,
            coordinatesFormat,
            numberFormat,
            numberFormat,
            nameof(self.X),
            nameof(self.Y),
            formatProvider
        );

    public static string FormatCartesianCoordinates<TCoordinates>(
        this TCoordinates self,
        string? format,
        IFormatProvider? formatProvider
    )
        where TCoordinates : ICartesianCoordinates
    {
        if (string.IsNullOrEmpty(format))
            return self.FormatCartesianCoordinates(null, null, formatProvider);

        var formats = format.Split(':');

        var coordinatesFormat = formats[0];
        var numberFormat = formats.Length > 1 ? formats[1] : null;
        return self.FormatCartesianCoordinates(coordinatesFormat, numberFormat, formatProvider);
    }

    public static string FormatPolarCoordinates<TCoordinates>(
        this TCoordinates self,
        string? coordinatesFormat,
        string? numberFormat,
        string? angleFormat,
        IFormatProvider? formatProvider
    )
        where TCoordinates : IPolarCoordinates =>
        FormatCoordinates(
            self,
            self.Phi,
            self.R,
            coordinatesFormat,
            angleFormat,
            numberFormat,
            nameof(self.Phi),
            nameof(self.R),
            formatProvider
        );

    public static string FormatPolarCoordinates<TCoordinates>(
        this TCoordinates self,
        string? format,
        IFormatProvider? formatProvider
    )
        where TCoordinates : IPolarCoordinates
    {
        if (string.IsNullOrEmpty(format))
            return self.FormatPolarCoordinates(null, null, null, formatProvider);

        var formats = format.Split(':');

        var coordinatesFormat = formats[0];
        var numberFormat = formats.Length > 1 ? formats[1] : null;
        var angleFormat = formats.Length > 2 ? formats[2] : null;

        return self.FormatPolarCoordinates(
            coordinatesFormat,
            numberFormat,
            $"{numberFormat}:{angleFormat}",
            formatProvider
        );
    }
}
