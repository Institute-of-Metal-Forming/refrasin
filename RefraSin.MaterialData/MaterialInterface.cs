namespace RefraSin.MaterialData;

/// <summary>
/// A record of material interface data.
/// </summary>
public record MaterialInterface(
        Guid From,
        Guid To,
        double InterfaceEnergy,
        double DiffusionCoefficient,
        double TransferCoefficient
    )
    : IMaterialInterface
{
    /// <inheritdoc />
    public Guid From { get; } = From;

    /// <inheritdoc />
    public Guid To { get; } = To;

    /// <inheritdoc />
    public double InterfaceEnergy { get; } = InterfaceEnergy;

    /// <inheritdoc />
    public double DiffusionCoefficient { get; } = DiffusionCoefficient;

    /// <inheritdoc />
    public double TransferCoefficient { get; } = TransferCoefficient;
}