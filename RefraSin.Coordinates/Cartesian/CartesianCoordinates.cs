using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using static System.Math;

namespace RefraSin.Coordinates.Cartesian;

/// <summary>
///     Abstract base class for coordinates in cartesian systems.
/// </summary>
public abstract class CartesianCoordinates : Coordinates, ICartesianCoordinates
{
    private  ICartesianCoordinateSystem? _system;

    /// <summary>
    ///     Creates the coordinates (0, 0).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    protected CartesianCoordinates(ICartesianCoordinateSystem? system)
    {
        _system = system;
    }

    /// <summary>
    ///     Creates the coordinates (x, y).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    protected CartesianCoordinates((double x, double y) coordinates, ICartesianCoordinateSystem? system = null) : this(system)
    {
        (X, Y) = coordinates;
    }

    /// <summary>
    ///     Creates the coordinates (x, y).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vertical coordinate</param>
    protected CartesianCoordinates(double x, double y, ICartesianCoordinateSystem? system = null) : this(system)
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

    /// <inheritdoc />
    public ICartesianCoordinateSystem System => _system ??= CartesianCoordinateSystem.Default;

    /// <summary>
    ///     Get the string representation of this instance.
    /// </summary>
    /// <param name="format">
    ///     combined format string "coordinatesFormat:numberFormat". See <see cref="ToString(String,String,IFormatProvider)" />
    ///     .
    /// </param>
    /// <param name="formatProvider">IFormatProvider</param>
    public override string ToString(string? format, IFormatProvider? formatProvider)
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

    /// <inheritdoc />
    public override double[] ToArray() => new[] { X, Y };
}