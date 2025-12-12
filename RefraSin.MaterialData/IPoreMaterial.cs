using RefraSin.Vertex;

namespace RefraSin.MaterialData;

public interface IPoreMaterial : IVertex
{
    ISubstanceProperties Substance { get; }
    double AverageParticleRadius { get; }
    double SurfaceEnergy { get; }
    double GrainBoundaryEnergy { get; }
    double InitialPorosity { get; }
    IViscoElasticProperties ViscoElastic { get; }
}
