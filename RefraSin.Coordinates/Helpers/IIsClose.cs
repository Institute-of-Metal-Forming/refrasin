namespace RefraSin.Coordinates.Helpers;

/// <summary>
/// Interface for equality determination within a specified precision.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IIsClose<in T>
    where T : IIsClose<T>
{
    /// <summary>
    /// Determines if two instances are equal with a specified precision.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public bool IsClose(T other, double precision = 1e-8);
}
