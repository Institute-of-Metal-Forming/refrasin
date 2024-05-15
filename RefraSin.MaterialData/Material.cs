namespace RefraSin.MaterialData;

/// <summary>
/// A record of material data.
/// </summary>
public record Material(
    Guid Id,
    string Name,
    double VolumeDiffusionCoefficient,
    double EquilibriumVacancyConcentration,
    double Density,
    double MolarMass,
    IInterfaceProperties Surface,
    IReadOnlyDictionary<Guid, IInterfaceProperties>? Interfaces = null
) : IMaterial
{
    /// <inheritdoc />
    public Guid Id { get; } = Id;

    /// <inheritdoc />
    public string Name { get; } = Name;

    /// <inheritdoc />
    public double VolumeDiffusionCoefficient { get; } = VolumeDiffusionCoefficient;

    /// <inheritdoc />
    public double EquilibriumVacancyConcentration { get; } = EquilibriumVacancyConcentration;

    /// <inheritdoc />
    public double Density { get; } = Density;

    /// <inheritdoc />
    public double MolarVolume { get; } = MolarMass / Density;

    /// <inheritdoc />
    public double MolarMass { get; } = MolarMass;

    /// <inheritdoc />
    public IInterfaceProperties Surface { get; } = Surface;

    /// <inheritdoc />
    public IReadOnlyDictionary<Guid, IInterfaceProperties> Interfaces { get; } = Interfaces ?? new Dictionary<Guid, IInterfaceProperties>();
}