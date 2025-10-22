using RefraSin.Vertex;

namespace RefraSin.MaterialData;

public interface IPoreMaterial : IVertex
{
    ISubstanceProperties Substance { get; }
    IViscoElasticProperties ViscoElastic { get; }
}
