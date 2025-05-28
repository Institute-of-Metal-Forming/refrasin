namespace RefraSin.MaterialData;

public record MatrixMaterial(
    Guid Id,
    ISubstanceProperties Substance,
    IViscoElasticProperties ViscoElastic
) : IMatrixMaterial { }
