using RefraSin.Coordinates;

namespace RefraSin.ParticleModel;

/// <summary>
/// Stellt ein double-Tupel mit Werten einer nach oben und nach unten vorhandenen Eigenschaft dar.
/// </summary>
public readonly record struct ToUpperToLower<TValue>(TValue ToUpper, TValue ToLower)
{
    /// <summary>
    /// Returns the components of the instance as 2-element array.
    /// </summary>
    public TValue[] ToArray() => [ToUpper, ToLower];

    public static implicit operator ToUpperToLower<TValue>(TValue both) => new(both, both);
    public static implicit operator ToUpperToLower<TValue>((TValue toUpper, TValue toLower) t) => new(t.toUpper, t.toLower);
}

public static class ToUpperToLowerExtensions
{
    public static double[] ToDoubleArray(this ToUpperToLower<Angle> self) => [self.ToUpper, self.ToLower];

    public static double Sum(this ToUpperToLower<double> self) => self.ToUpper + self.ToLower;
    
    public static Angle Sum(this ToUpperToLower<Angle> self) => self.ToUpper + self.ToLower;
}