using System;
using MathNet.Numerics;
using static System.Math;

namespace RefraSin.Coordinates.Polar;

/// <summary>
///     Abstract base class for coordinates in polar systems.
/// </summary>
public abstract class PolarCoordinates : Coordinates<PolarCoordinateSystem>, IFormattable
{
    private double _r;
    private Angle _phi;

    /// <summary>
    ///     Creates the coordinates (0, 0).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    protected PolarCoordinates(PolarCoordinateSystem? system) : base(system) { }

    /// <summary>
    ///     Creates the coordinates (phi, r).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    protected PolarCoordinates((Angle phi, double r) coordinates, PolarCoordinateSystem? system = null) : base(system)
    {
        (Phi, R) = coordinates;
    }

    /// <summary>
    ///     Creates the coordinates (phi, r).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    /// <param name="phi">angle coordinate</param>
    /// <param name="r">radius coordinate</param>
    protected PolarCoordinates(Angle phi, double r, PolarCoordinateSystem? system = null) : base(system)
    {
        Phi = phi;
        R = r;
    }

    /// <summary>
    ///     Gets or sets the angle coordinate.
    /// </summary>
    public Angle Phi
    {
        get
        {
            if (!_phi.IsInDomain(System.AngleReductionDomain))
                _phi = _phi.Reduce(System.AngleReductionDomain);
            return _phi;
        }
        set => _phi = value.Reduce(System.AngleReductionDomain);
    }

    /// <summary>
    ///     Gets or sets the radius coordinate.
    ///     <remarks>
    ///         Is always positive. Setting of negative values will cause to R > 0 and Phi += Pi.
    ///     </remarks>
    /// </summary>
    public double R
    {
        get => _r;
        set
        {
            if (value < 0)
                Phi += PI;
            _r = Abs(value);
        }
    }

    /// <summary>
    ///     Get the string representation of this instance.
    /// </summary>
    /// <param name="format">
    ///     combined format string "coordinatesFormat:numberFormat:angleFormat". See
    ///     <see cref="ToString(String,String,String,IFormatProvider)" />.
    /// </param>
    /// <param name="formatProvider">IFormatProvider</param>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format))
            return ToString(formatProvider);

        var formats = format.Split(':');

        var coordinatesFormat = formats[0];
        var numberFormat = formats.Length > 1 ? formats[1] : null;
        var angleFormat = formats.Length > 2 ? formats[2] : null;

        return ToString(coordinatesFormat, numberFormat, angleFormat, formatProvider);
    }

    /// <summary>
    ///     Get the string representation of this instance as "PolarCoordinates(X, Y)".
    /// </summary>
    public override string ToString() => ToString(null, null, null, null);

    /// <summary>
    ///     Get the string representation of this instance as "PolarCoordinates(X, Y)".
    /// </summary>
    /// <param name="formatProvider">IFormatProvider</param>
    public string ToString(IFormatProvider? formatProvider) => ToString(null, null, null, formatProvider);

    /// <summary>
    ///     Get the string representation of this instance.
    /// </summary>
    /// <param name="coordinatesFormat">format of the coordinates</param>
    /// <param name="numberFormat">
    ///     format of double numbers, see <see cref="Double.ToString(String,IFormatProvider)" />
    /// </param>
    /// <param name="angleFormat">
    ///     format for the angle <see cref="Angle.ToString(String,String,IFormatProvider)" />
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
    ///             <term> V </term> <description> be verbose, always add "Phi = " etc. </description>
    ///         </item>
    ///         <item>
    ///             <term> N </term> <description> add the type name in front </description>
    ///         </item>
    ///     </list>
    ///     It doesn't matter in which order the control characters are supplied.
    /// </remarks>
    public string ToString(string? coordinatesFormat, string? numberFormat, string? angleFormat, IFormatProvider? formatProvider) =>
        ToString(Phi, R, coordinatesFormat, $"{numberFormat}:{angleFormat}", numberFormat, nameof(Phi), nameof(R), formatProvider);

    /// <summary>
    ///     Gets a tuple of the coordinates.
    /// </summary>
    public (double Phi, double R) ToTuple() => (Phi, R);

    /// <summary>
    ///     Compute the angle distance between two sets of coordinates.
    /// </summary>
    /// <param name="other">other</param>
    /// <param name="allowNegative">whether to allow negative return values indicating direction</param>
    /// <returns>angle distance in interval [0; Pi], or [-Pi; Pi] if <paramref name="allowNegative" /> == true</returns>
    public Angle AngleTo(PolarCoordinates other, bool allowNegative = false)
    {
        if (System.Equals(other.System))
        {
            var diff = (other.Phi - Phi).Reduce(Angle.ReductionDomain.WithNegative);
            if (allowNegative) return diff;
            return Abs(diff);
        }

        throw new DifferentCoordinateSystemException(this, other);
    }

    /// <inheritdoc />
    public override void RotateBy(Angle angle)
    {
        Phi += angle;
    }

    /// <summary>
    ///     Base implementation of Equals().
    /// </summary>
    protected bool Equals(PolarCoordinates? other)
    {
        if (other == null) return false;
        if (R.AlmostEqual(0)) return other.R.AlmostEqual(0);
        if (System.Equals(other.System))
            return R.AlmostEqual(other.R) && Phi.Radians.AlmostEqual(other.Phi.Radians);
        return Equals((IVector) other);
    }

    /// <summary>
    ///     Base implementation of Equals() with precision.
    /// </summary>
    protected bool Equals(PolarCoordinates other, double precision)
    {
        if (R.AlmostEqual(0, precision)) return other.R.AlmostEqual(0, precision);
        if (System.Equals(other.System))
            return R.AlmostEqual(other.R, precision) && Phi.Radians.AlmostEqual(other.Phi.Radians, precision);
        return Equals((IVector) other);
    }

    /// <summary>
    ///     Base implementation of Equals() with precision.
    /// </summary>
    protected bool Equals(PolarCoordinates other, int digits)
    {
        if (R.AlmostEqual(0, digits)) return other.R.AlmostEqual(0, digits);
        if (System.Equals(other.System))
            return R.AlmostEqual(other.R, digits) && Phi.Radians.AlmostEqual(other.Phi.Radians, digits);
        return Equals((IVector) other);
    }
}