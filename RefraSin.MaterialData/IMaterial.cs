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
    /// Human readable name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Diffusion coefficient at free surfaces.
    /// </summary>
    double SurfaceDiffusionCoefficient { get; }

    /// <summary>
    /// Diffusion coefficient in bulk material.
    /// </summary>
    double BulkDiffusionCoefficient { get; }

    /// <summary>
    /// Vacancy concentration in equilibrium without effecting stresses.
    /// </summary>
    double EquilibriumVacancyConcentration { get; }

    /// <summary>
    /// Interface energy of free surfaces.
    /// </summary>
    double SurfaceEnergy { get; }

    /// <summary>
    /// Molar volume.
    /// </summary>
    double MolarVolume { get; }

    /// <summary>
    /// Density.
    /// </summary>
    double Density { get; }

    /// <summary>
    /// Molar weight.
    /// </summary>
    double MolarWeight { get; }
}