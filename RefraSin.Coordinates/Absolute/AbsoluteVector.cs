using System;
using MathNet.Numerics;
using RefraSin.Coordinates.Helpers;
using static System.Math;

namespace RefraSin.Coordinates.Absolute;

/// <summary>
///     Represents a vector in the absolute coordinate system (overall base system).
/// </summary>
public class AbsoluteVector : AbsoluteCoordinates, IVector, IPrecisionEquatable<AbsoluteVector>, ICloneable<AbsoluteVector>
{
    /// <summary>
    ///     Creates the absolute vector (0,0).
    /// </summary>
    public AbsoluteVector() { }

    /// <summary>
    ///     Creates the absolute vector (x, y).
    /// </summary>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vercircal coordinate</param>
    public AbsoluteVector(double x, double y) : base(x, y) { }

    /// <summary>
    ///     Creates the absolute vector (x, y).
    /// </summary>
    /// <param name="coordinates">tuple (x, y)</param>
    public AbsoluteVector((double x, double y) coordinates) : base(coordinates) { }

    /// <inheritdoc />
    public AbsoluteVector Clone() => new(X, Y);

    /// <inheritdoc />
    public bool Equals(AbsoluteVector other, double precision) => X.AlmostEqual(other.X, precision) && Y.AlmostEqual(other.Y, precision);

    /// <inheritdoc />
    public bool Equals(AbsoluteVector other, int digits) => X.AlmostEqual(other.X, digits) && Y.AlmostEqual(other.Y, digits);

    /// <inheritdoc />
    public bool Equals(AbsoluteVector? other) => other != null && X.AlmostEqual(other.X) && Y.AlmostEqual(other.Y);

    /// <inheritdoc />
    public AbsoluteVector Absolute => this;

    /// <inheritdoc />
    public double Norm => Sqrt(Pow(X, 2) + Pow(Y, 2));

    /// <inheritdoc />
    public IVector Add(IVector v)
    {
        if (v is AbsoluteVector av)
            return this + av;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public IVector Subtract(IVector v)
    {
        if (v is AbsoluteVector av)
            return this - av;
        throw new DifferentCoordinateSystemException(this, v);
    }

    /// <inheritdoc />
    public double ScalarProduct(IVector v)
    {
        if (v is AbsoluteVector av)
            return this * av;
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
    ///     supports all formats of <see cref="AbsoluteCoordinates.ToString(string, string, IFormatProvider)" />
    /// </remarks>
    public static AbsoluteVector Parse(string s)
    {
        var (value1, value2) = Parse(s, nameof(AbsoluteVector));
        return new AbsoluteVector(double.Parse(value1), double.Parse(value2));
    }

    /// <summary>
    ///     Negotiation (rotate by Pi).
    /// </summary>
    public static AbsoluteVector operator -(AbsoluteVector v) => new(-v.X, -v.Y);

    /// <summary>
    ///     Vectorial addition. See <see cref="Add" />.
    /// </summary>
    public static AbsoluteVector operator +(AbsoluteVector v1, AbsoluteVector v2) =>
        new(v1.X + v2.X, v1.Y + v2.Y);

    /// <summary>
    ///     Vectorial subtraction. See <see cref="Subtract" />.
    /// </summary>
    public static AbsoluteVector operator -(AbsoluteVector v1, AbsoluteVector v2) =>
        new(v1.X - v2.X, v1.Y - v2.Y);

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static AbsoluteVector operator *(double d, AbsoluteVector v) =>
        new(d * v.X, d * v.Y);

    /// <summary>
    ///     Scales the vector. See <see cref="ScaleBy" />.
    /// </summary>
    public static AbsoluteVector operator *(AbsoluteVector v, double d) => d * v;

    /// <summary>
    ///     Scalar product. See <see cref="ScalarProduct" />.
    /// </summary>
    public static double operator *(AbsoluteVector v1, AbsoluteVector v2) => v1.X * v2.X + v1.Y * v2.Y;
}