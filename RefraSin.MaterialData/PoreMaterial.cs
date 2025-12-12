namespace RefraSin.MaterialData;

public record PoreMaterial(
    Guid Id,
    ISubstanceProperties Substance,
    double AverageParticleRadius,
    double SurfaceEnergy,
    double GrainBoundaryEnergy,
    double InitialPorosity,
    IViscoElasticProperties ViscoElastic
) : IPoreMaterial { }
