using System;
using MathNet.Numerics;
using static System.Math;

namespace RefraSin.Coordinates.Cartesian;

/// <summary>
///     Abstract base class for coordinates in cartesian systems.
/// </summary>
public abstract class CartesianCoordinates : Coordinates<CartesianCoordinateSystem>, IFormattable
{
    /// <summary>
    ///     Creates the coordinates (0, 0).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    protected CartesianCoordinates(CartesianCoordinateSystem? system) : base(system) { }

    /// <summary>
    ///     Creates the coordinates (x, y).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    protected CartesianCoordinates((double x, double y) coordinates, CartesianCoordinateSystem? system = null) : base(system)
    {
        (X, Y) = coordinates;
    }

    /// <summary>
    ///     Creates the coordinates (x, y).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vertical coordinate</param>
    protected CartesianCoordinates(double x, double y, CartesianCoordinateSystem? system = null) : base(system)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Gets or sets the horizontal coordinate X.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    ///     Gets or sets the vertical coordinate Y.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    ///     Get the string representation of this instance.
    /// </summary>
    /// <param name="format">
    ///     combined format string "coordinatesFormat:numberFormat". See <see cref="ToString(String,String,IFormatProvider)" />
    ///     .
    /// </param>
    /// <param name="formatProvider">IFormatProvider</param>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format))
            return ToString(formatProvider);

        var formats = format.Split(':');

        var coordinatesFormat = formats[0];
        var numberFormat = formats.Length > 1 ? formats[1] : null;

        return ToString(coordinatesFormat, numberFormat, formatProvider);
    }

    /// <inheritdoc />
    public sealed override void RotateBy(Angle angle)
    {
        (X, Y) = (X * Cos(angle) - Y * Sin(angle), Y * Cos(angle) + X * Sin(angle));
    }

    /// <summary>
    ///     Get the string representation of this instance as "CartesianCoordinates(X, Y)".
    /// </summary>
    public override string ToString() => ToString(null, null, null);

    /// <summary>
    ///     Get the string representation of this instance as "CartesianCoordinates(X, Y)".
    /// </summary>
    /// <param name="formatProvider">IFormatProvider</param>
    public string ToString(IFormatProvider? formatProvider) => ToString(null, null, formatProvider);

    /// <summary>
    ///     Get the string representation of this instance.
    /// </summary>
    /// <param name="coordinatesFormat">format of the coordinates</param>
    /// <param name="numberFormat">
    ///     format of double numbers, see <see cref="Double.ToString(String,IFormatProvider)" />
    /// </param>
    /// <param name="formatProvider">IFormatProvider</param>
    /// <remarks>
    ///     valid control characters in <paramref name="coordinatesFormat" /> are
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
    ///             <term> N </term> <description> add the type name in front </description>
    ///         </item>
    ///     </list>
    ///     It doesn't matter in which order the control characters are supplied.
    /// </remarks>
    public string ToString(string? coordinatesFormat, string? numberFormat, IFormatProvider? formatProvider) =>
        ToString(X, Y, coordinatesFormat, numberFormat, numberFormat, nameof(X), nameof(Y), formatProvider);

    /// <summary>
    ///     Gets a tuple of the coordinates.
    /// </summary>
    public (double X, double Y) ToTuple() => (X, Y);

    /// <summary>
    ///     Base implementation of Equals() with precision.
    /// </summary>
    protected bool Equals(CartesianCoordinates other, double precision)
    {
        if (System.Equals(other.System))
            return X.AlmostEqual(other.X, precision) && Y.AlmostEqual(other.Y, precision);
        return Equals((IPoint) other);
    }

    /// <summary>
    ///     Base implementation of Equals() with precision.
    /// </summary>
    protected bool Equals(CartesianCoordinates other, int digits)
    {
        if (System.Equals(other.System))
            return X.AlmostEqual(other.X, digits) && Y.AlmostEqual(other.Y, digits);
        return Equals((IPoint) other);
    }

    /// <summary>
    ///     Base implementation of Equals().
    /// </summary>
    protected bool Equals(CartesianCoordinates? other)
    {
        if (other == null) return false;
        if (System.Equals(other.System))
            return X.AlmostEqual(other.X) && Y.AlmostEqual(other.Y);
        return Equals((IPoint) other);
    }
}