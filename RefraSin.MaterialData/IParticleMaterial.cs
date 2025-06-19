using RefraSin.Vertex;

namespace RefraSin.MaterialData;

/// <summary>
/// Interface for types providing material data.
/// </summary>
public interface IParticleMaterial : IVertex
{
    /// <summary>
    /// Human-readable name.
    /// </summary>
    string Name { get; }

    ISubstanceProperties Substance { get; }

    IInterfaceProperties Surface { get; }

    IReadOnlyDictionary<Guid, IInterfaceProperties> Interfaces { get; }
}
