using RefraSin.Coordinates.Cartesian;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Absolute;

/// <summary>
///     Represents a point in the absolute coordinate system (overall base system).
/// </summary>
public readonly struct AbsolutePoint(double x, double y)
    : ICartesianPoint,
        IIsClose<AbsolutePoint>,
        IPointArithmetics<AbsolutePoint, AbsoluteVector>
{
    /// <summary>
    ///     Creates the absolute point (0,0).
    /// </summary>
    public AbsolutePoint()
        : this(0, 0) { }

    /// <summary>
    ///     Creates the absolute point (x, y).
    /// </summary>
    /// <param name="coordinates">tuple (x, y)</param>
    public AbsolutePoint((double x, double y) coordinates)
        : this(coordinates.x, coordinates.y) { }

    /// <inheritdoc />
    public double X { get; } = x;

    /// <inheritdoc />
    public double Y { get; } = y;

    /// <inheritdoc />
    public ICartesianCoordinateSystem System => CartesianCoordinateSystem.Default;

    /// <inheritdoc />
    public AbsolutePoint Absolute => this;

    public AbsolutePoint AddVector(IVector v)
    {
        var abs = v.Absolute;
        return new AbsolutePoint(X + abs.X, Y + abs.Y);
    }

    ICartesianPoint IPointOperations<ICartesianPoint, ICartesianVector>.AddVector(
        ICartesianVector v
    ) => AddVector(v);

    AbsolutePoint IPointOperations<AbsolutePoint, AbsoluteVector>.AddVector(AbsoluteVector v) =>
        AddVector(v);

    public AbsoluteVector VectorTo(IPoint p)
    {
        var abs = p.Absolute;
        return new AbsoluteVector(X - abs.X, Y - abs.Y);
    }

    ICartesianVector IPointOperations<ICartesianPoint, ICartesianVector>.VectorTo(
        ICartesianPoint p
    ) => VectorTo(p);

    AbsoluteVector IPointOperations<AbsolutePoint, AbsoluteVector>.VectorTo(AbsolutePoint p) =>
        VectorTo(p);

    /// <inheritdoc />
    public double DistanceTo(IPoint other)
    {
        var abs = other.Absolute;
        return Sqrt(Pow((X - abs.X) * System.XScale, 2) + Pow((Y - abs.Y) * System.YScale, 2));
    }

    double IPointOperations<ICartesianPoint, ICartesianVector>.DistanceTo(ICartesianPoint p) =>
        DistanceTo(p);

    double IPointOperations<AbsolutePoint, AbsoluteVector>.DistanceTo(AbsolutePoint p) =>
        DistanceTo(p);

    /// <summary>
    ///     Computes the point halfway on the straight line between two points.
    /// </summary>
    /// <param name="other">other point</param>
    public AbsolutePoint Centroid(IPoint other)
    {
        var abs = other.Absolute;
        return new AbsolutePoint(0.5 * (X + abs.X), 0.5 * (Y + abs.Y));
    }

    AbsolutePoint IPointOperations<AbsolutePoint, AbsoluteVector>.Centroid(AbsolutePoint other) =>
        Centroid(other);

    ICartesianPoint IPointOperations<ICartesianPoint, ICartesianVector>.Centroid(
        ICartesianPoint other
    ) => Centroid(other);

    /// <inheritdoc />
    public bool IsClose(AbsolutePoint other, double precision = 1e-8) =>
        X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);

    public bool IsClose(ICartesianPoint other, double precision = 1e-8)
    {
        if (System.Equals(other.System))
            return X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

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
    public static AbsolutePoint operator +(AbsolutePoint p, AbsoluteVector v) => p.AddVector(v);

    /// <summary>
    ///     Subtraction of a vector from a point, see <see cref="AddVector" />.
    /// </summary>
    public static AbsolutePoint operator -(AbsolutePoint p, AbsoluteVector v) => p + -v;

    /// <summary>
    ///     Computes the vector between two points. See <see cref="VectorTo" />.
    /// </summary>
    public static AbsoluteVector operator -(AbsolutePoint p1, AbsolutePoint p2) => p1.VectorTo(p2);

    /// <inheritdoc />
    public double[] ToArray() => [X, Y];

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.FormatCartesianCoordinates(format, formatProvider);

    public override string ToString() => ToString(null, null);

    /// <inheritdoc />
    public AbsolutePoint ScaleBy(double scale) => new(scale * X, scale * Y);

    /// <inheritdoc />
    public AbsolutePoint RotateBy(double rotation) =>
        new(X * Cos(rotation) - Y * Sin(rotation), Y * Cos(rotation) + X * Sin(rotation));

    public AbsolutePoint MoveBy(double dx, double dy) => new(X + dx, Y + dy);

    public AbsolutePoint ScaleBy(double scaleX, double scaleY) => new(scaleX * X, scaleY * Y);

    public static implicit operator AbsolutePoint((double x, double y) t) => new(t.x, t.y);
}
