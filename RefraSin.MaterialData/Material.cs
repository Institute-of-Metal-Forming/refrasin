namespace RefraSin.MaterialData;

/// <summary>
/// A record of material data.
/// </summary>
public record Material(
    Guid Id,
    string Name,
    double SurfaceDiffusionCoefficient,
    double BulkDiffusionCoefficient,
    double EquilibriumVacancyConcentration,
    double SurfaceEnergy,
    double Density,
    double MolarWeight
) : IMaterial
{
    /// <inheritdoc />
    public Guid Id { get; } = Id;

    /// <inheritdoc />
    public string Name { get; } = Name;

    /// <inheritdoc />
    public double SurfaceDiffusionCoefficient { get; } = SurfaceDiffusionCoefficient;

    /// <inheritdoc />
    public double BulkDiffusionCoefficient { get; } = BulkDiffusionCoefficient;

    /// <inheritdoc />
    public double EquilibriumVacancyConcentration { get; } = EquilibriumVacancyConcentration;

    /// <inheritdoc />
    public double SurfaceEnergy { get; } = SurfaceEnergy;

    /// <inheritdoc />
    public double MolarVolume { get; } = MolarWeight / Density;

    /// <inheritdoc />
    public double Density { get; } = Density;

    /// <inheritdoc />
    public double MolarWeight { get; } = MolarWeight;
}