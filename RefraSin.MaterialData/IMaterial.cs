namespace RefraSin.MaterialData;

/// <summary>
/// Interface for types providing material data.
/// </summary>
public interface IMaterial : IBulkProperties, ISubstanceProperties
{
    /// <summary>
    /// Unique id.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Human readable name.
    /// </summary>
    string Name { get; }

    IInterfaceProperties Surface { get; }
    
    IReadOnlyDictionary<Guid, IInterfaceProperties> Interfaces { get; }
}