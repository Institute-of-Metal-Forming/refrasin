namespace RefraSin.MaterialData;

/// <summary>
/// Interface for types providing material data.
/// </summary>
public interface IMaterial
{
    /// <summary>
    /// Unique id.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Human-readable name.
    /// </summary>
    string Name { get; }

    IBulkProperties Bulk { get; }

    ISubstanceProperties Substance { get; }

    IInterfaceProperties Surface { get; }

    IReadOnlyDictionary<Guid, IInterfaceProperties> Interfaces { get; }
}
