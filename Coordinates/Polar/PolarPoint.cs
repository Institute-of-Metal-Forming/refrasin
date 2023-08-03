using System;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Polar;

/// <summary>
///     Stellt einen Punkt in Polarkoordinaten dar.
/// </summary>
public class PolarPoint : PolarCoordinates, ICloneable<PolarPoint>, IPoint, IPrecisionEquatable<PolarPoint>
{
    /// <summary>
    ///     Creates the point (0, 0).
    /// </summary>
    public PolarPoint() : base(null) { }

    /// <summary>
    ///     Creates the point (0, 0).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public PolarPoint(PolarCoordinateSystem? system) : base(system) { }

    /// <summary>
    ///     Creates the point (phi, r).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public PolarPoint((Angle phi, double r) coordinates, PolarCoordinateSystem? system = null) : base(coordinates, system) { }

    /// <summary>
    ///     Creates the point (phi, r).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    /// <param name="phi">angle coordinate</param>
    /// <param name="r">radius coordinate</param>
    public PolarPoint(Angle phi, double r, PolarCoordinateSystem? system = null) : base(phi, r, system) { }

    /// <summary>
    ///     Creates a point based on a template. The coordinates systems are automatically castd.
    /// </summary>
    /// <param name="other">template</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public PolarPoint(IPoint other, PolarCoordinateSystem? system = null) : base(system)
    {
        var absoluteCoordinates = other.Absolute;
        var originCoordinates = System.Origin.Absolute;
        var x = absoluteCoordinates.X - originCoordinates.X;
        var y = absoluteCoordinates.Y - originCoordinates.Y;
        R = Sqrt(Pow(x, 2) + Pow(y, 2)) / System.RScale;
        Phi = (y > 0 ? Acos(x / R) : y < 0 ? PI + Acos(-x / R) : x >= 0 ? 0 : PI) - System.RotationAngle;
    }

    /// <inheritdoc />
    public PolarPoint Clone() => new(Phi, R, System);

    /// <inheritdoc />
    public AbsolutePoint Absolute
    {
        get
        {
            var origin = System.Origin.Absolute;
            return new AbsolutePoint(origin.X + R * System.RScale * Cos(Phi + System.RotationAngle),
                origin.Y + R * System.RScale * Sin(Phi + System.RotationAngle));
        }
    }

    /// <inheritdoc />
    public IPoint AddVector(IVector v)
    {
        if (v is PolarVector pv)
            return this + pv;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public IVector VectorTo(IPoint p)
    {
        if (p is PolarPoint pp)
            return pp - this;
        throw new DifferentCoordinateSystemException(this, p);
    }

    IPoint ICloneable<IPoint>.Clone() => Clone();

    /// <inheritdoc />
    public double DistanceTo(IPoint other)
    {
        if (other is PolarPoint p)
            if (System.Equals(p.System))
                return CosLaw.C(R, p.R, Abs(p.Phi - Phi));
        throw new DifferentCoordinateSystemException(this, other);
    }

    /// <inheritdoc />
    public bool Equals(PolarPoint? other) => Equals((PolarCoordinates?) other);

    /// <inheritdoc />
    public bool Equals(PolarPoint other, double precision) => Equals((PolarCoordinates) other, precision);

    /// <inheritdoc />
    public bool Equals(PolarPoint other, int digits) => Equals((PolarCoordinates) other, digits);

    /// <summary>
    ///     Computes the point halfway on the straight line between two points.
    /// </summary>
    /// <param name="other">other point</param>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public PolarPoint PointHalfWayTo(PolarPoint other)
    {
        if (System.Equals(other.System))
        {
            var angle = (other.Phi - Phi).Reduce(Angle.ReductionDomain.WithNegative);
            var dist = CosLaw.C(R, other.R, Abs(angle));
            var s = Sqrt(2 * (Pow(R, 2) + Pow(other.R, 2)) - Pow(dist, 2)) / 2;
            return new PolarPoint(Phi + Sign(angle) * CosLaw.Gamma(R, s, dist / 2), s, System);
        }

        throw new DifferentCoordinateSystemException(this, other);
    }

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="PolarCoordinates.ToString(string, string, string, IFormatProvider)" />
    /// </remarks>
    public static PolarPoint Parse(string s)
    {
        var (value1, value2) = Parse(s, nameof(PolarPoint));
        return new PolarPoint(Angle.Parse(value1), double.Parse(value2));
    }

    /// <summary>
    ///     Addition of a vector to a point, see <see cref="AddVector" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static PolarPoint operator +(PolarPoint p, PolarVector v)
    {
        if (p.System.Equals(v.System))
        {
            var x = p.R * Cos(p.Phi) + v.R * Cos(v.Phi);
            var y = p.R * Sin(p.Phi) + v.R * Sin(v.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi = y > 0 ? Acos(x / r) : y < 0 ? PI + Acos(-x / r) : x >= 0 ? 0 : PI;

            return new PolarPoint(phi, r, p.System);
        }

        throw new DifferentCoordinateSystemException(p, v);
    }

    /// <summary>
    ///     Addition of a vector to a point, see <see cref="AddVector" />.
    /// </summary>
    public static PolarPoint operator +(PolarVector v, PolarPoint p) => p + v;

    /// <summary>
    ///     Subtraction of a vector from a point, see <see cref="AddVector" />.
    /// </summary>
    public static PolarPoint operator -(PolarPoint p, PolarVector v) => p + -v;

    /// <summary>
    ///     Computes the vector between two points. See <see cref="VectorTo" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static PolarVector operator -(PolarPoint p1, PolarPoint p2)
    {
        if (p1.System.Equals(p2.System))
        {
            var x = p1.R * Cos(p1.Phi) - p2.R * Cos(p2.Phi);
            var y = p1.R * Sin(p1.Phi) - p2.R * Sin(p2.Phi);

            var r = Sqrt(Pow(x, 2) + Pow(y, 2));
            var phi = y > 0 ? Acos(x / r) : y < 0 ? PI + Acos(-x / r) : x >= 0 ? 0 : PI;

            return new PolarVector(phi, r, p1.System);
        }

        throw new DifferentCoordinateSystemException(p1, p2);
    }
}