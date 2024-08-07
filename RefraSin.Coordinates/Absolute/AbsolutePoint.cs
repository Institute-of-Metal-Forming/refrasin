using RefraSin.Coordinates.Cartesian;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Absolute;

/// <summary>
///     Represents a point in the absolute coordinate system (overall base system).
/// </summary>
public readonly struct AbsolutePoint
    : ICartesianPoint,
        IIsClose<AbsolutePoint>,
        IPointArithmetics<AbsolutePoint, AbsoluteVector>
{
    /// <summary>
    ///     Creates the absolute point (0,0).
    /// </summary>
    public AbsolutePoint()
    {
        X = 0;
        Y = 0;
    }

    /// <summary>
    ///     Creates the absolute point (x, y).
    /// </summary>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vercircal coordinate</param>
    public AbsolutePoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Creates the absolute point (x, y).
    /// </summary>
    /// <param name="coordinates">tuple (x, y)</param>
    public AbsolutePoint((double x, double y) coordinates)
    {
        (X, Y) = coordinates;
    }

    /// <inheritdoc />
    public double X { get; }

    /// <inheritdoc />
    public double Y { get; }

    /// <inheritdoc />
    public ICartesianCoordinateSystem System => CartesianCoordinateSystem.Default;

    /// <inheritdoc />
    public AbsolutePoint Absolute => this;

    /// <inheritdoc />
    public AbsolutePoint AddVector(AbsoluteVector v) => this + v;

    /// <inheritdoc />
    public AbsoluteVector VectorTo(AbsolutePoint p) => p - this;

    /// <inheritdoc />
    public double DistanceTo(AbsolutePoint other) =>
        Sqrt(Pow(X - other.X, 2) + Pow(Y - other.Y, 2));

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
    public IPoint Centroid(IPoint other) => Centroid(other.Absolute);

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="AbsolutePoint.ToString(string, IFormatProvider)" />
    /// </remarks>
    public static AbsolutePoint Parse(string s)
    {
        var (value1, value2) = s.ParseCoordinateString(nameof(AbsolutePoint));
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
    public static AbsolutePoint operator -(AbsolutePoint self) => new(-self.X, -self.Y);

    /// <inheritdoc />
    public bool IsClose(ICartesianPoint other, double precision = 1e-8)
    {
        if (System.Equals(other.System))
            return X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

    /// <inheritdoc />
    public double[] ToArray() => [X, Y];

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.FormatCartesianCoordinates(format, formatProvider);

    /// <inheritdoc />
    public AbsolutePoint ScaleBy(double scale) => new(scale * X, scale * Y);

    /// <inheritdoc />
    public AbsolutePoint RotateBy(double rotation) =>
        new(X * Cos(rotation) - Y * Sin(rotation), Y * Cos(rotation) + X * Sin(rotation));

    public AbsolutePoint MoveBy(double dx, double dy) => new(X + dx, Y + dy);

    public AbsolutePoint ScaleBy(double scaleX, double scaleY) => new(scaleX * X, scaleY * Y);
}
