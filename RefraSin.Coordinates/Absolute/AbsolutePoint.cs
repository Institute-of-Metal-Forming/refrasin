using RefraSin.Coordinates.Cartesian;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Absolute;

/// <summary>
///     Represents a point in the absolute coordinate system (overall base system).
/// </summary>
public class AbsolutePoint
    : AbsoluteCoordinates,
        ICartesianPoint,
        ICloneable<AbsolutePoint>,
        IIsClose<AbsolutePoint>,
        IPointArithmetics<AbsolutePoint, AbsoluteVector>
{
    /// <summary>
    ///     Creates the absolute point (0,0).
    /// </summary>
    public AbsolutePoint() { }

    /// <summary>
    ///     Creates the absolute point (x, y).
    /// </summary>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vercircal coordinate</param>
    public AbsolutePoint(double x, double y)
        : base(x, y) { }

    /// <summary>
    ///     Creates the absolute point (x, y).
    /// </summary>
    /// <param name="coordinates">tuple (x, y)</param>
    public AbsolutePoint((double x, double y) coordinates)
        : base(coordinates) { }

    /// <inheritdoc />
    public AbsolutePoint Clone() => new(X, Y);

    /// <inheritdoc />
    public AbsolutePoint Absolute => this;

    /// <inheritdoc />
    public IPoint AddVector(IVector v)
    {
        if (v is AbsoluteVector av)
            return this + av;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public IVector VectorTo(IPoint p)
    {
        if (p is AbsolutePoint ap)
            return ap - this;
        throw new DifferentCoordinateSystemException(this, p);
    }

    IPoint ICloneable<IPoint>.Clone() => Clone();

    /// <inheritdoc />
    public double DistanceTo(IPoint other)
    {
        if (other is AbsolutePoint a)
            return Sqrt(Pow(X - a.X, 2) + Pow(Y - a.Y, 2));
        throw new DifferentCoordinateSystemException(this, other);
    }

    /// <inheritdoc />
    public bool IsClose(AbsolutePoint other, double precision = 1e-8) =>
        X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);

    /// <summary>
    ///     Computes the point halfway on the straight line between two points.
    /// </summary>
    /// <param name="other">other point</param>
    /// <returns></returns>
    public AbsolutePoint Centroid(AbsolutePoint other) =>
        new(0.5 * (X + other.X), 0.5 * (Y + other.Y));

    /// <summary>
    ///     Computes the point halfway on the straight line between two points.
    /// </summary>
    /// <param name="other">other point</param>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public AbsolutePoint Centroid(IPoint other) => Centroid(other.Absolute);

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="AbsoluteCoordinates.ToString(string, string, IFormatProvider)" />
    /// </remarks>
    public static AbsolutePoint Parse(string s)
    {
        var (value1, value2) = Parse(s, nameof(AbsolutePoint));
        return new AbsolutePoint(double.Parse(value1), double.Parse(value2));
    }

    /// <summary>
    ///     Addition of a vector to a point, see <see cref="AddVector" />.
    /// </summary>
    public static AbsolutePoint operator +(AbsolutePoint p, AbsoluteVector v) =>
        new(p.X + v.X, p.Y + v.Y);

    /// <summary>
    ///     Addition of a vector to a point, see <see cref="AddVector" />.
    /// </summary>
    public static AbsolutePoint operator +(AbsoluteVector v, AbsolutePoint p) => p + v;

    /// <summary>
    ///     Subtraction of a vector from a point, see <see cref="AddVector" />.
    /// </summary>
    public static AbsolutePoint operator -(AbsolutePoint p, AbsoluteVector v) => p + -v;

    /// <summary>
    ///     Computes the vector between two points. See <see cref="VectorTo" />.
    /// </summary>
    public static AbsoluteVector operator -(AbsolutePoint p1, AbsolutePoint p2) =>
        new(p1.X - p2.X, p1.Y - p2.Y);

    /// <inheritdoc />
    public bool IsClose(ICartesianPoint other, double precision = 1e-8)
    {
        if (System.Equals(other.System))
            return X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }
}
