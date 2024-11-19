using System;

namespace RefraSin.Enumerables;

/// <summary>
///     Exception that is thrown, if a non-nullable property was not set explicitly and is therefore in an invalid state.
///     Used to avoid <see cref="NullReferenceException" /> with the benefit of a clearer message and further information
///     about location and reason.
/// </summary>
public class PropertyNotSetException : InvalidOperationException
{
    /// <inheritdoc cref="PropertyNotSetException" />
    /// <param name="propertyName">name of the affected property</param>
    /// <param name="ownerName">name of the owning type or object</param>
    public PropertyNotSetException(string propertyName, string ownerName)
        : base(
            $"The property '{propertyName}' of '{ownerName}' is not set, nor a default value can be obtained."
        )
    {
        PropertyName = propertyName;
        OwnerName = ownerName;
    }

    /// <summary>
    ///     Name of the affected property.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    ///     Name of the owning type or object.
    /// </summary>
    public string OwnerName { get; }
}
