using RefraSin.Vertex;

namespace RefraSin.MaterialData;

public interface IMatrixMaterial : IVertex
{
    ISubstanceProperties Substance { get; }
    IViscoElasticProperties ViscoElastic { get; }
}
