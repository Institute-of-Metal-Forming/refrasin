namespace RefraSin.Core.Materials
{
    /// <summary>
    /// Data class containing material data.
    /// </summary>
    /// <param name="Label">label for human recognition</param>
    /// <param name="MolarVolume">molar volume</param>
    /// <param name="SurfaceEnergy">energy bound in free surfaces per area</param>
    /// <param name="SurfaceDiffusionCoefficient">diffusion coefficient on free surfaces</param>
    /// <param name="ThermalVacancyConcentration">fraction of thermal vacancies in lattice</param>
    public record Material(string Label, double MolarVolume, double SurfaceEnergy, double SurfaceDiffusionCoefficient,
        double ThermalVacancyConcentration = 1e-4);
}