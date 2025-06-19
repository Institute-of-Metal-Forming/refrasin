namespace RefraSin.MaterialData;

public interface IMatrixMaterial
{
    ISubstanceProperties SubstanceProperties { get; }
    IViscoElasticProperties ViscoElasticProperties { get; }
}
