using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Cartesian;

/// <summary>
///     Represents a point in cartesian coordinate system.
/// </summary>
public readonly struct CartesianPoint(double x, double y, ICartesianCoordinateSystem? system = null)
    : ICartesianPoint,
        IPointArithmetics<CartesianPoint, CartesianVector>
{
    /// <summary>
    ///     Creates the point (0, 0) in the default system.
    /// </summary>
    public CartesianPoint(ICartesianCoordinateSystem? system = null)
        : this(0, 0, system) { }

    /// <summary>
    ///     Creates the point (x, y).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianPoint(
        (double x, double y) coordinates,
        ICartesianCoordinateSystem? system = null
    )
        : this(coordinates.x, coordinates.y, system) { }

    /// <summary>
    ///     Creates a point based on a template. The coordinates systems are automatically castd.
    /// </summary>
    /// <param name="other">template</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianPoint(IPoint other, ICartesianCoordinateSystem? system = null)
        : this(system)
    {
        var origin = System.Origin.Absolute;
        var transformed = other
            .Absolute.MoveBy(-origin.X, -origin.Y)
            .RotateBy(-System.RotationAngle)
            .ScaleBy(1 / System.XScale, 1 / System.YScale);
        X = transformed.X;
        Y = transformed.Y;
    }

    /// <inheritdoc />
    public double X { get; } = x;

    /// <inheritdoc />
    public double Y { get; } = y;

    /// <inheritdoc />
    public ICartesianCoordinateSystem System { get; } = system ?? CartesianCoordinateSystem.Default;

    /// <inheritdoc />
    public AbsolutePoint Absolute
    {
        get
        {
            var origin = System.Origin.Absolute;
            return new AbsolutePoint(X, Y)
                .ScaleBy(System.XScale, System.YScale)
                .RotateBy(System.RotationAngle)
                .MoveBy(origin.X, origin.Y);
        }
    }

    public CartesianPoint AddVector(ICartesianVector v)
    {
        if (System.Equals(v.System))
            return new CartesianPoint(X + v.X, Y + v.Y, System);
        throw new DifferentCoordinateSystemException(this, v);
    }

    ICartesianPoint IPointOperations<ICartesianPoint, ICartesianVector>.AddVector(
        ICartesianVector v
    ) => AddVector(v);

    CartesianPoint IPointOperations<CartesianPoint, CartesianVector>.AddVector(CartesianVector v) =>
        AddVector(v);

    public CartesianVector VectorTo(ICartesianPoint p)
    {
        if (System.Equals(p.System))
            return new CartesianVector(X - p.X, Y - p.Y, System);
        throw new DifferentCoordinateSystemException(this, p);
    }

    ICartesianVector IPointOperations<ICartesianPoint, ICartesianVector>.VectorTo(
        ICartesianPoint p
    ) => VectorTo(p);

    CartesianVector IPointOperations<CartesianPoint, CartesianVector>.VectorTo(CartesianPoint p) =>
        VectorTo(p);

    /// <inheritdoc />
    public double DistanceTo(ICartesianPoint other)
    {
        if (System.Equals(other.System))
            return Sqrt(
                Pow((X - other.X) * System.XScale, 2) + Pow((Y - other.Y) * System.YScale, 2)
            );
        return Absolute.DistanceTo(other.Absolute);
    }

    double IPointOperations<CartesianPoint, CartesianVector>.DistanceTo(CartesianPoint p) =>
        DistanceTo(p);

    /// <inheritdoc />
    public bool IsClose(ICartesianPoint other, double precision = 1e-8)
    {
        if (System.Equals(other.System))
            return X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

    /// <summary>
    ///     Computes the point halfway on the straight line between two points.
    /// </summary>
    /// <param name="other">other point</param>
    public CartesianPoint Centroid(ICartesianPoint other)
    {
        if (System.Equals(other.System))
            return new CartesianPoint(0.5 * (X + other.X), 0.5 * (Y + other.Y));
        return new CartesianPoint(Absolute.Centroid(other.Absolute), System);
    }

    CartesianPoint IPointOperations<CartesianPoint, CartesianVector>.Centroid(
        CartesianPoint other
    ) => Centroid(other);

    ICartesianPoint IPointOperations<ICartesianPoint, ICartesianVector>.Centroid(
        ICartesianPoint other
    ) => Centroid(other);

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="CartesianCoordinates.ToString(string, string, IFormatProvider)" />
    /// </remarks>
    public static CartesianPoint Parse(string s)
    {
        var (value1, value2) = s.ParseCoordinateString(nameof(CartesianPoint));
        return new CartesianPoint(double.Parse(value1), double.Parse(value2));
    }

    /// <summary>
    ///     Addition of a vector to a point, see <see cref="AddVector" />.
    /// </summary>
    /// ///
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static CartesianPoint operator +(CartesianPoint p, CartesianVector v) => p.AddVector(v);

    /// <summary>
    ///     Computes the vector between two points. See <see cref="VectorTo" />.
    /// </summary>
    public static CartesianVector operator -(CartesianPoint p1, CartesianPoint p2) =>
        p1.VectorTo(p2);

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.FormatCartesianCoordinates(format, formatProvider);

    public override string ToString() => ToString(null, null);

    /// <inheritdoc />
    public double[] ToArray() => [X, Y];

    /// <inheritdoc />
    public CartesianPoint ScaleBy(double scale) => new(scale * X, scale * Y);

    /// <inheritdoc />
    public CartesianPoint RotateBy(double rotation) =>
        new(X * Cos(rotation) - Y * Sin(rotation), Y * Cos(rotation) + X * Sin(rotation));
}
