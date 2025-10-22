namespace RefraSin.MaterialData;

public record PoreMaterial(
    Guid Id,
    ISubstanceProperties Substance,
    IViscoElasticProperties ViscoElastic
) : IPoreMaterial { }
