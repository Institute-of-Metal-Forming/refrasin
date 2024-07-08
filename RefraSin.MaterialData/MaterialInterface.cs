namespace RefraSin.MaterialData;

/// <summary>
/// A record of material interface data.
/// </summary>
public record MaterialInterface(
    Guid From,
    Guid To,
    IInterfaceProperties Properties)
    : IMaterialInterface;