namespace RefraSin.MaterialData;

public record BulkProperties(
    double VolumeDiffusionCoefficient,
    double EquilibriumVacancyConcentration
) : IBulkProperties { }
