using System;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Polar;

/// <summary>
///     Stellt einen Vektor in Polarkoordinaten dar.
/// </summary>
public class PolarVector : PolarCoordinates, ICloneable<PolarVector>, IPolarVector
{
    /// <summary>
    ///     Creates the vector (0, 0).
    /// </summary>
    public PolarVector() : base(null) { }

    /// <summary>
    ///     Creates the vector (0, 0).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public PolarVector(IPolarCoordinateSystem? system) : base(system) { }

    /// <summary>
    ///     Creates the vector (phi, r).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public PolarVector((Angle phi, double r) coordinates, IPolarCoordinateSystem? system = null) : base(coordinates, system) { }

    /// <summary>
    ///     Creates the vector (phi, r).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    /// <param name="phi">angle coordinate</param>
    /// <param name="r">radius coordinate</param>
    public PolarVector(Angle phi, double r, IPolarCoordinateSystem? system = null) : base(phi, r, system) { }

    /// <summary>
    ///     Creates a vector based on a template. The coordinates systems are automatically castd.
    /// </summary>
    /// <param name="other">template</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public PolarVector(IVector other, IPolarCoordinateSystem? system = null) : base(system)
    {
        var absoluteCoordinates = other.Absolute;
        var x = absoluteCoordinates.X;
        var y = absoluteCoordinates.Y;
        R = Sqrt(Pow(x, 2) + Pow(y, 2)) / System.RScale;
        Phi = (y > 0 ? Acos(x / R) : y < 0 ? PI + Acos(-x / R) : x >= 0 ? 0 : PI) - System.RotationAngle;
    }

    /// <inheritdoc />
    public PolarVector Clone() => new(Phi, R, System);

    /// <inheritdoc />
    public bool IsClose(IPolarVector other, double precision = 1e-8)
    {
        if (System.Equals(other.System))
            return R.IsClose(other.R, precision) && Phi.IsClose(other.Phi, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

    /// <inheritdoc />
    public AbsoluteVector Absolute =>
        new(R * System.RScale * Cos(Phi + System.RotationAngle),
            R * System.RScale * Sin(Phi + System.RotationAngle));

    /// <inheritdoc />
    public double Norm => R;

    /// <inheritdoc />
    public IVector Add(IVector v)
    {
        if (v is PolarVector pv)
            return this + pv;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public IVector Subtract(IVector v)
    {
        if (v is PolarVector pv)
            return this - pv;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public double ScalarProduct(IVector v)
    {
        if (v is PolarVector pv)
            return this * pv;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public void ScaleBy(double scale)
    {
        R *= scale;
    }

    IVector ICloneable<IVector>.Clone() => Clone();

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="PolarCoordinates.ToString(string, string, string, IFormatProvider)" />
    /// </remarks>
    public static PolarVector Parse(string s)
    {
        var (value1, value2) = Parse(s, nameof(PolarVector));
        return new PolarVector(Angle.Parse(value1), double.Parse(value2));
    }

    /// <summary>
    ///     Vectorial addition. See <see cref="Add" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static PolarVector operator +(PolarVector v1, PolarVector v2)
    {
        if (v1.System.Equals(v2.System))
        {
            var x = v1.R * Cos(v1.Phi) + v2.R * Cos(v2.Phi);
            var y = v1.R * Sin(v1.Phi) + v2.R * Sin(v2.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi = double.Atan2(y, x);
            return new PolarVector(phi, r, v1.System);
        }

        throw new DifferentCoordinateSystemException(v1, v2);
    }

    /// <summary>
    ///     Vectorial subtraction. See <see cref="Subtract" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static PolarVector operator -(PolarVector v1, PolarVector v2)
    {
        if (v1.System.Equals(v2.System))
        {
            var x = v1.R * Cos(v1.Phi) - v2.R * Cos(v2.Phi);
            var y = v1.R * Sin(v1.Phi) - v2.R * Sin(v2.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi = y > 0 ? Acos(x / r) : y < 0 ? PI + Acos(-x / r) : x >= 0 ? 0 : PI;

            return new PolarVector(phi, r, v1.System);
        }

        throw new DifferentCoordinateSystemException(v1, v2);
    }

    /// <summary>
    ///     Negotiation (rotate by Pi).
    /// </summary>
    public static PolarVector operator -(PolarVector v) => new(v.Phi + PI, v.R);

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static PolarVector operator *(double d, PolarVector v) =>
        new(v.Phi, d * v.R, v.System);

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static PolarVector operator *(PolarVector v, double d) => d * v;

    /// <summary>
    ///     Scalar product. See <see cref="ScalarProduct" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static double operator *(PolarVector v1, PolarVector v2)
    {
        if (v1.System == v2.System)
            return v1.R * v2.R * Cos(Abs(v1.Phi - v2.Phi));
        throw new DifferentCoordinateSystemException(v1, v2);
    }
}