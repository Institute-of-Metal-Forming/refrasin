using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Cartesian;

/// <summary>
///     Stellt einen Vektor im kartesischen Koordinatensystem dar.
/// </summary>
public readonly struct CartesianVector(
    double x,
    double y,
    ICartesianCoordinateSystem? system = null
) : ICartesianVector, IVectorArithmetics<CartesianVector>
{
    /// <summary>
    ///     Creates the vector (0, 0) in the default system.
    /// </summary>
    public CartesianVector(ICartesianCoordinateSystem? system = null)
        : this(0, 0, system) { }

    /// <summary>
    ///     Creates the vector (x, y).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianVector(
        (double x, double y) coordinates,
        ICartesianCoordinateSystem? system = null
    )
        : this(coordinates.x, coordinates.y, system) { }

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
    public double X { get; } = x;

    /// <inheritdoc />
    public double Y { get; } = y;

    /// <inheritdoc />
    public ICartesianCoordinateSystem System { get; } = system ?? CartesianCoordinateSystem.Default;

    /// <inheritdoc />
    public AbsoluteVector Absolute =>
        new AbsoluteVector(X, Y)
            .ScaleBy(System.XScale, System.YScale)
            .RotateBy(System.RotationAngle);

    /// <inheritdoc />
    public double Norm => Sqrt(Pow(X, 2) + Pow(Y, 2));

    /// <inheritdoc />
    public bool IsClose(ICartesianVector other, double precision)
    {
        if (System.Equals(other.System))
            return X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

    public CartesianVector Add(ICartesianVector v)
    {
        if (System == v.System)
            return new CartesianVector(X + v.X, Y + v.Y, System);
        throw new DifferentCoordinateSystemException(this, v);
    }

    ICartesianVector IVectorOperations<ICartesianVector>.Add(ICartesianVector v) => Add(v);

    CartesianVector IVectorOperations<CartesianVector>.Add(CartesianVector v) => Add(v);

    /// <inheritdoc />
    public CartesianVector Reverse() => new(-X, -Y);

    ICartesianVector IVectorOperations<ICartesianVector>.Reverse() => Reverse();

    /// <inheritdoc />
    public double ScalarProduct(ICartesianVector v)
    {
        if (System == v.System)
            return X * v.X + Y * v.Y;
        throw new DifferentCoordinateSystemException(this, v);
    }

    double IVectorOperations<CartesianVector>.ScalarProduct(CartesianVector v) => ScalarProduct(v);

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
    public static CartesianVector operator -(CartesianVector v) => v.Reverse();

    /// <summary>
    ///     Vectorial addition. See <see cref="Add" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static CartesianVector operator +(CartesianVector v1, CartesianVector v2) => v1.Add(v2);

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static CartesianVector operator *(double d, CartesianVector v) => v.ScaleBy(d);

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static CartesianVector operator *(CartesianVector v, double d) => d * v;

    /// <summary>
    ///     Scalar product. See <see cref="ScalarProduct" />.
    /// </summary>
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static double operator *(CartesianVector v1, CartesianVector v2) => v1.ScalarProduct(v2);

    /// <inheritdoc />
    public static CartesianVector operator /(CartesianVector left, double right) =>
        left.ScaleBy(1 / right);

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        this.FormatCartesianCoordinates(format, formatProvider);

    public override string ToString() => ToString(null, null);

    /// <inheritdoc />
    public double[] ToArray() => [X, Y];

    /// <inheritdoc />
    public CartesianVector ScaleBy(double scale) => new(scale * X, scale * Y);

    /// <inheritdoc />
    public CartesianVector RotateBy(double rotation) =>
        new(X * Cos(rotation) - Y * Sin(rotation), Y * Cos(rotation) + X * Sin(rotation));

    /// <inheritdoc />
    public static CartesianVector operator -(CartesianVector left, CartesianVector right) =>
        left + -right;
    
    public static implicit operator CartesianVector((double x, double y) t) => new(t.x,t.y);
}
