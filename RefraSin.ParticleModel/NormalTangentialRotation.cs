using RefraSin.Coordinates;
using static System.Math;

namespace RefraSin.ParticleModel;

/// <summary>
/// Structure encapsulating quantities with three components, one in normal direction, one in tangential and one for rotation.
/// </summary>
public readonly record struct NormalTangentialRotation<TValue>(TValue Normal, TValue Tangential, TValue Rotation)
{
    /// <summary>
    /// Returns the components of the instance as 3-element array.
    /// </summary>
    public TValue[] ToArray() => new[] { Normal, Tangential, Rotation };

    public static implicit operator NormalTangentialRotation<TValue>((TValue Normal, TValue Tangential, TValue Rotation) t) =>
        new(t.Normal, t.Tangential, t.Rotation);
}

public static class NormalTangentialRotationExtensions
{
    public static double[] ToDoubleArray(this NormalTangentialRotation<Angle> self) => new double[] { self.Normal, self.Tangential };
}