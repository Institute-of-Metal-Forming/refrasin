namespace RefraSin.MaterialData;

public record PoreMaterial(
    Guid Id,
    ISubstanceProperties Substance,
    double AverageParticleRadius,
    double InterfaceEnergy,
    IViscoElasticProperties ViscoElastic
) : IPoreMaterial { }
