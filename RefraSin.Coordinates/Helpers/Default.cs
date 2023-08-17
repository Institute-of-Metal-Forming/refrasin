namespace RefraSin.Coordinates.Helpers;

/// <summary>
///     Static class serving default instances of other classes.
///     The instance can be gotten with <see cref="Instance" />.
///     The instance is <em>not</em> a singleton.
/// </summary>
/// <typeparam name="T">type of the class to serve an instance for</typeparam>
public static class Default<T> where T : new()
{
    private static T? _default;

    /// <summary>
    ///     Get the instance.
    ///     On first call the instance is created.
    /// </summary>
    public static T Instance => _default ??= new T();
}