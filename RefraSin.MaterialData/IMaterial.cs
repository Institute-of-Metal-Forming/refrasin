namespace RefraSin.MaterialData;

public interface IMaterial
{
    Guid Id { get; }

    string Name { get; }

    double SurfaceDiffusionCoefficient { get; }

    double BulkDiffusionCoefficient { get; }

    double EquilibriumVacancyConcentration { get; }

    double SurfaceEnergy { get; }

    double MolarVolume { get; }

    double Density { get; }

    double MolarWeight { get; set; }
}