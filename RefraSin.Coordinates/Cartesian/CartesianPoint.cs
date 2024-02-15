using System;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Cartesian;

/// <summary>
///     Represents a point in cartesian coordinate system.
/// </summary>
public class CartesianPoint : CartesianCoordinates, IPoint, ICloneable<CartesianPoint>, IIsClose<CartesianPoint>
{
    /// <summary>
    ///     Creates the point (0, 0) in the default system.
    /// </summary>
    public CartesianPoint() : base(null) { }

    /// <summary>
    ///     Creates the point (0, 0).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianPoint(CartesianCoordinateSystem? system) : base(system) { }

    /// <summary>
    ///     Creates the point (x, y).
    /// </summary>
    /// <param name="coordinates">tuple of coordinates</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianPoint((double x, double y) coordinates, CartesianCoordinateSystem? system = null) : base(coordinates, system) { }

    /// <summary>
    ///     Creates the point (x, y).
    /// </summary>
    /// <param name="system">coordinate system, if null the default system is used</param>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vertical coordinate</param>
    public CartesianPoint(double x, double y, CartesianCoordinateSystem? system = null) : base(x, y, system) { }

    /// <summary>
    ///     Creates a point based on a template. The coordinates systems are automatically castd.
    /// </summary>
    /// <param name="other">template</param>
    /// <param name="system">coordinate system, if null the default system is used</param>
    public CartesianPoint(IPoint other, CartesianCoordinateSystem? system = null) : base(system)
    {
        var absoluteCoords = other.Absolute;
        var baseCoords = System.Origin.Absolute;
        X = absoluteCoords.X - baseCoords.X;
        Y = absoluteCoords.Y - baseCoords.Y;
        RotateBy(-System.RotationAngle);
        X /= System.XScale;
        Y /= System.YScale;
        System = System;
    }

    /// <inheritdoc />
    public CartesianPoint Clone() => new(X, Y, System);

    /// <inheritdoc />
    public AbsolutePoint Absolute
    {
        get
        {
            var absolute = new AbsolutePoint(X * System.XScale, Y * System.YScale);
            absolute.RotateBy(System.RotationAngle);
            var origin = System.Origin.Absolute;
            absolute.X += origin.X;
            absolute.Y += origin.Y;
            return absolute;
        }
    }

    /// <inheritdoc />
    public IPoint AddVector(IVector v)
    {
        if (v is CartesianVector cv)
            return this + cv;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public IVector VectorTo(IPoint p)
    {
        if (p is CartesianPoint cp)
            return cp - this;
        throw new DifferentCoordinateSystemException(this, p);
    }

    IPoint ICloneable<IPoint>.Clone() => Clone();

    /// <inheritdoc />
    public double DistanceTo(IPoint other)
    {
        if (other is CartesianCoordinates c)
            if (System.Equals(c.System))
                return Sqrt(Pow((X - c.X) * System.XScale, 2) + Pow((Y - c.Y) * System.YScale, 2));
        throw new DifferentCoordinateSystemException(this, other);
    }

    /// <inheritdoc />
    public bool IsClose(CartesianPoint other, double precision = 1e-8)
    {
        if (System.Equals(other.System))
            return X.IsClose(other.X, precision) && Y.IsClose(other.Y, precision);
        return Absolute.IsClose(other.Absolute, precision);
    }

    /// <summary>
    ///     Computes the point halfway on the straight line between two points.
    /// </summary>
    /// <param name="other">other point</param>
    public CartesianPoint PointHalfWayTo(CartesianPoint other)
    {
        if (System.Equals(other.System))
            return new CartesianPoint(0.5 * (X + other.X), 0.5 * (Y + other.Y));
        throw new DifferentCoordinateSystemException(this, other);
    }

    /// <summary>
    ///     Parse from string representation.
    /// </summary>
    /// <param name="s">string to parse</param>
    /// <remarks>
    ///     supports all formats of <see cref="CartesianCoordinates.ToString(string, string, IFormatProvider)" />
    /// </remarks>
    public static CartesianPoint Parse(string s)
    {
        var (value1, value2) = Parse(s, nameof(CartesianPoint));
        return new CartesianPoint(double.Parse(value1), double.Parse(value2));
    }

    /// <summary>
    ///     Addition of a vector to a point, see <see cref="AddVector" />.
    /// </summary>
    /// ///
    /// <exception cref="DifferentCoordinateSystemException">if systems are not equal</exception>
    public static CartesianPoint operator +(CartesianPoint p, CartesianVector v)
    {
        if (p.System.Equals(v.System))
            return new CartesianPoint(p.X + v.X, p.Y + v.Y, p.System);
        throw new DifferentCoordinateSystemException(p, v);
    }

    /// <summary>
    ///     Addition of a vector to a point, see <see cref="AddVector" />.
    /// </summary>
    public static CartesianPoint operator +(CartesianVector v, CartesianPoint p) => p + v;

    /// <summary>
    ///     Subtraction of a vector from a point, see <see cref="AddVector" />.
    /// </summary>
    public static CartesianPoint operator -(CartesianPoint p, CartesianVector v) => p + -v;

    /// <summary>
    ///     Computes the vector between two points. See <see cref="VectorTo" />.
    /// </summary>
    public static CartesianVector operator -(CartesianPoint p1, CartesianPoint p2)
    {
        if (p1.System.Equals(p2.System))
            return new CartesianVector(p1.X - p2.X, p1.Y - p2.Y, p1.System);
        throw new DifferentCoordinateSystemException(p1, p2);
    }
}