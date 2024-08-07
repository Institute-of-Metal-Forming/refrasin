using System;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Cartesian;

/// <summary>
///     Stellt einen Vektor im kartesischen Koordinatensystem dar.
/// </summary>
public class CartesianVector
    : CartesianCoordinates,
        ICartesianVector,
        ICloneable<CartesianVector>,
        IVectorArithmetics<CartesianPoint, CartesianVector>
{
    /// <summary>
    ///     Creates the vector (0, 0) in the default system.
    /// </summary>
    public CartesianVector()
        : base(null) { }

    /// <summary>
    ///     Creates the vector (0, 0).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianVector(CartesianCoordinateSystem? system)
        : base(system) { }

    /// <summary>
    ///     Creates the vector (x, y).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianVector(
        (double x, double y) coordinates,
        ICartesianCoordinateSystem? system = null
    )
        : base(coordinates, system) { }

    /// <summary>
    ///     Creates the vector (x, y).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vertical coordinate</param>
    public CartesianVector(double x, double y, ICartesianCoordinateSystem? system = null)
        : base(x, y, system) { }

    /// <summary>
    ///     Creates a vector based on a template. The coordinates systems are automatically castd.
    /// </summary>
    /// <param name="other">template</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianVector(IVector other, ICartesianCoordinateSystem? system = null)
        : base(system)
    {
        var absoluteCoords = other.Absolute;
        X = absoluteCoords.X;
        Y = absoluteCoords.Y;
        RotateBy(-System.RotationAngle);
        X /= System.XScale;
        Y /= System.YScale;
    }

    /// <inheritdoc />
    public CartesianVector Clone() => new(X, Y, System);

    /// <inheritdoc />
    public bool IsClose(ICartesianVector other, double precision)
    {
        if (System.Equals(other.System))
            return X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

    /// <inheritdoc />
    public AbsoluteVector Absolute
    {
        get
        {
            var absolute = new AbsoluteVector(X * System.XScale, Y * System.YScale);
            absolute.RotateBy(System.RotationAngle);
            return absolute;
        }
    }

    /// <inheritdoc />
    public double Norm => Sqrt(Pow(X, 2) + Pow(Y, 2));

    /// <inheritdoc />
    public IVector Add(IVector v)
    {
        if (v is CartesianVector cv)
            return this + cv;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public IVector Subtract(IVector v)
    {
        if (v is CartesianVector cv)
            return this - cv;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public double ScalarProduct(IVector v)
    {
        if (v is CartesianVector cv)
            return this * cv;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public void ScaleBy(double scale)
    {
        X *= scale;
        Y *= scale;
    }

    IVector ICloneable<IVector>.Clone() => Clone();

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="CartesianCoordinates.ToString(string, string, IFormatProvider)" />
    /// </remarks>
    public static CartesianVector Parse(string s)
    {
        var (value1, value2) = Parse(s, nameof(CartesianVector));
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
}
