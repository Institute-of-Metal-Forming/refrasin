namespace RefraSin.MaterialData;

/// <summary>
/// A record of material data.
/// </summary>
public record ParticleMaterial(
    Guid Id,
    string Name,
    ISubstanceProperties Substance,
    IInterfaceProperties Surface,
    IReadOnlyDictionary<Guid, IInterfaceProperties>? Interfaces = null
) : IParticleMaterial
{
    /// <inheritdoc />
    public IReadOnlyDictionary<Guid, IInterfaceProperties> Interfaces { get; } =
        Interfaces ?? new Dictionary<Guid, IInterfaceProperties>();
}
