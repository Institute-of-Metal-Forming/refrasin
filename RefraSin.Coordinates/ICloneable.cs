namespace RefraSin.Coordinates;

/// <summary>
///     Interface defining a method for shallow cloning.
/// </summary>
/// <typeparam name="T">type of the cloneable object</typeparam>
public interface ICloneable<out T>
{
    /// <summary>
    ///     Returns a shallow copy of this instance.
    /// </summary>
    public T Clone();
}