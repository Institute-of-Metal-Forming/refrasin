using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Cartesian;

/// <summary>
///     Stellt einen Vektor im kartesischen Koordinatensystem dar.
/// </summary>
public readonly struct CartesianVector : ICartesianVector, IVectorArithmetics<CartesianVector>
{
    /// <summary>
    ///     Creates the vector (0, 0) in the default system.
    /// </summary>
    public CartesianVector(ICartesianCoordinateSystem? system = null)
    {
        X = 0;
        Y = 0;
        System = system ?? CartesianCoordinateSystem.Default;
    }

    /// <summary>
    ///     Creates the vector (x, y).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianVector(
        (double x, double y) coordinates,
        ICartesianCoordinateSystem? system = null
    )
        : this(system)
    {
        (X, Y) = coordinates;
    }

    /// <summary>
    ///     Creates the vector (x, y).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vertical coordinate</param>
    public CartesianVector(double x, double y, ICartesianCoordinateSystem? system = null)
        : this(system)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Creates a vector based on a template. The coordinates systems are automatically castd.
    /// </summary>
    /// <param name="other">template</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianVector(IVector other, ICartesianCoordinateSystem? system = null)
        : this(system)
    {
        var transformed = other
            .Absolute.ScaleBy(1 / System.XScale, 1 / System.YScale)
            .RotateBy(System.RotationAngle);
        X = transformed.X;
        Y = transformed.Y;
    }

    /// <inheritdoc />
    public double X { get; }

    /// <inheritdoc />
    public double Y { get; }

    /// <inheritdoc />
    public ICartesianCoordinateSystem System { get; }

    /// <inheritdoc />
    public bool IsClose(ICartesianVector other, double precision)
    {
        if (System.Equals(other.System))
            return X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

    /// <inheritdoc />
    public AbsoluteVector Absolute =>
        new AbsoluteVector(X, Y)
            .ScaleBy(System.XScale, System.YScale)
            .RotateBy(System.RotationAngle);

    /// <inheritdoc />
    public double Norm => Sqrt(Pow(X, 2) + Pow(Y, 2));

    /// <inheritdoc />
    public CartesianVector Add(CartesianVector v) => this + v;

    /// <inheritdoc />
    public CartesianVector Subtract(CartesianVector v) => this - v;

    /// <inheritdoc />
    public double ScalarProduct(CartesianVector v) => this * v;

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="CartesianCoordinates.ToString(string, string, IFormatProvider)" />
    /// </remarks>
    public static CartesianVector Parse(string s)
    {
        var (value1, value2) = s.ParseCoordinateString(nameof(CartesianVector));
        return new CartesianVector(double.Parse(value1), double.Parse(value2));
    }

    /// <summary>
    ///     Negotiation (rotate by Pi).
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static CartesianVector operator -(CartesianVector v) => new(-v.X, -v.Y);

    /// <summary>
    ///     Vectorial addition. See <see cref="Add" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static CartesianVector operator +(CartesianVector v1, CartesianVector v2)
    {
        if (v1.System == v2.System)
            return new CartesianVector(v1.X + v2.X, v1.Y + v2.Y, v1.System);
        throw new DifferentCoordinateSystemException(v1, v2);
    }

    /// <summary>
    ///     Vectorial subtacrtion. See <see cref="Subtract" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static CartesianVector operator -(CartesianVector v1, CartesianVector v2)
    {
        if (v1.System == v2.System)
            return new CartesianVector(v1.X - v2.X, v1.Y - v2.Y, v1.System);
        throw new DifferentCoordinateSystemException(v1, v2);
    }

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static CartesianVector operator *(double d, CartesianVector v) =>
        new(d * v.X, d * v.Y, v.System);

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static CartesianVector operator *(CartesianVector v, double d) => d * v;

    /// <summary>
    ///     Scalar product. See <see cref="ScalarProduct" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static double operator *(CartesianVector v1, CartesianVector v2)
    {
        if (v1.System == v2.System)
            return v1.X * v2.X + v1.Y * v2.Y;
        throw new DifferentCoordinateSystemException(v1, v2);
    }

    /// <inheritdoc />
    public static CartesianVector operator /(CartesianVector left, double right) =>
        new(left.X / right, left.Y / right);

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.FormatCartesianCoordinates(format, formatProvider);

    /// <inheritdoc />
    public double[] ToArray() => [X, Y];

    /// <inheritdoc />
    public CartesianVector ScaleBy(double scale) => scale * this;

    /// <inheritdoc />
    public CartesianVector RotateBy(double rotation) =>
        new(X * Cos(rotation) - Y * Sin(rotation), Y * Cos(rotation) + X * Sin(rotation));
}
