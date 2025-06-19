using RefraSin.Vertex;

namespace RefraSin.MaterialData;

public interface IMatrixMaterial : IVertex
{
    ISubstanceProperties SubstanceProperties { get; }
    IViscoElasticProperties ViscoElasticProperties { get; }
}
