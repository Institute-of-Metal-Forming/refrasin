namespace RefraSin.MaterialData;

public record MatrixMaterial(
    ISubstanceProperties SubstanceProperties,
    IViscoElasticProperties ViscoElasticProperties
) : IMatrixMaterial { }
