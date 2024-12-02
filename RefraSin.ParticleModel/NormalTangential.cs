using System.Numerics;
using RefraSin.Coordinates;

namespace RefraSin.ParticleModel;

/// <summary>
/// Structure encapsulating quantities with two components, one in normal direction, one in tangential.
/// </summary>
public readonly record struct NormalTangential<TValue>(TValue Normal, TValue Tangential)
    : IMultiplyOperators<NormalTangential<TValue>, double, NormalTangential<TValue>>,
        IDivisionOperators<NormalTangential<TValue>, double, NormalTangential<TValue>>
    where TValue : IAdditionOperators<TValue, TValue, TValue>,
        IMultiplyOperators<TValue, double, TValue>,
        IDivisionOperators<TValue, double, TValue>
{
    /// <summary>
    /// Returns the components of the instance as 2-element array.
    /// </summary>
    public TValue[] ToArray() => [Normal, Tangential];

    public static implicit operator NormalTangential<TValue>(TValue both) => new(both, both);

    public static implicit operator NormalTangential<TValue>(
        (TValue Normal, TValue Tangential) t
    ) => new(t.Normal, t.Tangential);

    /// <inheritdoc />
    public static NormalTangential<TValue> operator *(
        NormalTangential<TValue> left,
        double right
    ) => new(left.Normal * right, left.Tangential * right);

    /// <inheritdoc />
    public static NormalTangential<TValue> operator /(
        NormalTangential<TValue> left,
        double right
    ) => new(left.Normal / right, left.Tangential / right);
}

public static class NormalTangentialExtensions
{
    public static double[] ToDoubleArray(this NormalTangential<Angle> self) =>
        new double[] { self.Normal, self.Tangential };

    public static NormalTangential<double> ToDoubleValued(this NormalTangential<Angle> self) =>
        new(self.Normal, self.Tangential);

    public static NormalTangential<Angle> ToAngleValued(this NormalTangential<double> self) =>
        new(self.Normal, self.Tangential);
}
