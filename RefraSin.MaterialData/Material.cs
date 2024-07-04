namespace RefraSin.MaterialData;

/// <summary>
/// A record of material data.
/// </summary>
public record Material(
    Guid Id,
    string Name,
    IBulkProperties Bulk,
    ISubstanceProperties Substance,
    IInterfaceProperties Surface,
    IReadOnlyDictionary<Guid, IInterfaceProperties>? Interfaces = null
) : IMaterial
{
    /// <inheritdoc />
    public IReadOnlyDictionary<Guid, IInterfaceProperties> Interfaces { get; } = Interfaces ?? new Dictionary<Guid, IInterfaceProperties>();
}