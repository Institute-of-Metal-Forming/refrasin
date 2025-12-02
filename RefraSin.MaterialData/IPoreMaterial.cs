using RefraSin.Vertex;

namespace RefraSin.MaterialData;

public interface IPoreMaterial : IVertex
{
    ISubstanceProperties Substance { get; }
    double AverageParticleRadius { get; }
    double InterfaceEnergy { get; }
    IViscoElasticProperties ViscoElastic { get; }
}
