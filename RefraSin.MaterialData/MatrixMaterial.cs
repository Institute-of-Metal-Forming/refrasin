namespace RefraSin.MaterialData;

public record MatrixMaterial(
    Guid Id,
    ISubstanceProperties SubstanceProperties,
    IViscoElasticProperties ViscoElasticProperties
) : IMatrixMaterial { }
