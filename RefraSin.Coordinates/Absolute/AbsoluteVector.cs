using RefraSin.Coordinates.Cartesian;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Absolute;

/// <summary>
///     Represents a vector in the absolute coordinate system (overall base system).
/// </summary>
public readonly struct AbsoluteVector(double x, double y)
    : ICartesianVector,
        IIsClose<AbsoluteVector>,
        IVectorArithmetics<AbsoluteVector>
{
    /// <summary>
    ///     Creates the absolute vector (0,0).
    /// </summary>
    public AbsoluteVector()
        : this(0, 0) { }

    /// <summary>
    ///     Creates the absolute vector (x, y).
    /// </summary>
    /// <param name="coordinates">tuple (x, y)</param>
    public AbsoluteVector((double x, double y) coordinates)
        : this(coordinates.x, coordinates.y) { }

    /// <inheritdoc />
    public double X { get; } = x;

    /// <inheritdoc />
    public double Y { get; } = y;

    /// <inheritdoc />
    public ICartesianCoordinateSystem System => CartesianCoordinateSystem.Default;

    /// <inheritdoc />
    public bool IsClose(AbsoluteVector other, double precision = 1e-8) =>
        X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);

    /// <inheritdoc />
    public AbsoluteVector Absolute => this;

    /// <inheritdoc />
    public double Norm => Sqrt(Pow(X, 2) + Pow(Y, 2));

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="AbsoluteVector.ToString(string, IFormatProvider)" />
    /// </remarks>
    public static AbsoluteVector Parse(string s)
    {
        var (value1, value2) = s.ParseCoordinateString(nameof(AbsoluteVector));
        return new AbsoluteVector(double.Parse(value1), double.Parse(value2));
    }

    /// <summary>
    ///     Negotiation (rotate by Pi).
    /// </summary>
    public static AbsoluteVector operator -(AbsoluteVector v) => new(-v.X, -v.Y);

    /// <summary>
    ///     Vectorial addition.
    /// </summary>
    public static AbsoluteVector operator +(AbsoluteVector v1, AbsoluteVector v2) =>
        new(v1.X + v2.X, v1.Y + v2.Y);

    /// <summary>
    ///     Vectorial subtraction.
    /// </summary>
    public static AbsoluteVector operator -(AbsoluteVector v1, AbsoluteVector v2) =>
        new(v1.X - v2.X, v1.Y - v2.Y);

    /// <summary>
    ///     Scales the vector.
    /// </summary>
    public static AbsoluteVector operator *(double d, AbsoluteVector v) => new(d * v.X, d * v.Y);

    /// <summary>
    ///     Scales the vector.
    /// </summary>
    public static AbsoluteVector operator *(AbsoluteVector v, double d) => d * v;

    /// <summary>
    ///     Scalar product.
    /// </summary>
    public static double operator *(AbsoluteVector v1, AbsoluteVector v2) =>
        v1.X * v2.X + v1.Y * v2.Y;

    /// <inheritdoc />
    public bool IsClose(ICartesianVector other, double precision)
    {
        if (System.Equals(other.System))
            return X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

    /// <inheritdoc />
    public static AbsoluteVector operator /(AbsoluteVector left, double right) =>
        new(left.X / right, left.Y / right);

    /// <inheritdoc />
    public double[] ToArray() => [X, Y];

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.FormatCartesianCoordinates(format, formatProvider);

    /// <inheritdoc />
    public AbsoluteVector Add(AbsoluteVector v) => this + v;

    /// <inheritdoc />
    public AbsoluteVector Subtract(AbsoluteVector v) => this - v;

    /// <inheritdoc />
    public double ScalarProduct(AbsoluteVector v) => this * v;

    /// <inheritdoc />
    public AbsoluteVector ScaleBy(double scale) => scale * this;

    /// <inheritdoc />
    public AbsoluteVector RotateBy(double rotation) =>
        new(X * Cos(rotation) - Y * Sin(rotation), Y * Cos(rotation) + X * Sin(rotation));

    public AbsoluteVector ScaleBy(double scaleX, double scaleY) => new(scaleX * X, scaleY * Y);
}
