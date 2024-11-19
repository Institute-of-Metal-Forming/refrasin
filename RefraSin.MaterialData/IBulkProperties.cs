namespace RefraSin.MaterialData;

public interface IBulkProperties
{
    /// <summary>
    /// Diffusion coefficient in bulk material.
    /// </summary>
    double VolumeDiffusionCoefficient { get; }

    /// <summary>
    /// Vacancy concentration in equilibrium without effecting stresses.
    /// </summary>
    double EquilibriumVacancyConcentration { get; }
}
