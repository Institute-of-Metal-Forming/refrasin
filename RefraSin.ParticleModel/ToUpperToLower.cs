using System.Numerics;
using RefraSin.Coordinates;

namespace RefraSin.ParticleModel;

/// <summary>
/// Stellt ein double-Tupel mit Werten einer nach oben und nach unten vorhandenen Eigenschaft dar.
/// </summary>
public readonly record struct ToUpperToLower<TValue>(TValue ToUpper, TValue ToLower)
    : IMultiplyOperators<ToUpperToLower<TValue>, double, ToUpperToLower<TValue>>,
        IDivisionOperators<ToUpperToLower<TValue>, double, ToUpperToLower<TValue>>
    where TValue : IAdditionOperators<TValue, TValue, TValue>,
        IMultiplyOperators<TValue, double, TValue>,
        IDivisionOperators<TValue, double, TValue>
{
    /// <summary>
    /// Returns the components of the instance as 2-element array.
    /// </summary>
    public TValue[] ToArray() => [ToUpper, ToLower];

    public static implicit operator ToUpperToLower<TValue>(TValue both) => new(both, both);

    public static implicit operator ToUpperToLower<TValue>((TValue toUpper, TValue toLower) t) =>
        new(t.toUpper, t.toLower);

    public TValue Sum => ToUpper + ToLower;

    /// <inheritdoc />
    public static ToUpperToLower<TValue> operator *(ToUpperToLower<TValue> left, double right) =>
        new(left.ToUpper * right, left.ToLower * right);

    /// <inheritdoc />
    public static ToUpperToLower<TValue> operator /(ToUpperToLower<TValue> left, double right) =>
        new(left.ToUpper / right, left.ToLower / right);
}

public static class ToUpperToLowerExtensions
{
    public static double[] ToDoubleArray(this ToUpperToLower<Angle> self) =>
        [self.ToUpper, self.ToLower];
}
