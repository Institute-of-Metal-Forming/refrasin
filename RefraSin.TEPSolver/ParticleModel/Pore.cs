using RefraSin.MaterialData;
using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
using RefraSin.Vertex;

namespace RefraSin.TEPSolver.ParticleModel;

public class Pore : IPore<NodeBase>, IPorePorosity, IPoreElasticStrain, IPoreDenseVolume
{
    public Pore(
        IPore<INode> pore,
        SolutionState solutionState,
        double porosity,
        double elasticStrain,
        IPoreMaterial poreMaterial
    )
    {
        Id = pore.Id;
        Nodes = pore.Nodes.Select(n => solutionState.Nodes[n.Id]).ToReadOnlyVertexCollection();
        Porosity = porosity;
        ElasticStrain = elasticStrain;
        Volume = this.Volume<Pore, NodeBase>();
        PoreMaterial = poreMaterial;
        DenseVolume = Volume * (1 - Porosity);
    }

    private Pore(
        SolutionState solutionState,
        Pore previousState,
        StepVector stepVector,
        double timeStepWidth
    )
    {
        Id = previousState.Id;
        Nodes = previousState
            .Nodes.Select(n => solutionState.Nodes[n.Id])
            .ToReadOnlyVertexCollection();
        Porosity =
            previousState.Porosity
            + stepVector.ItemValue<PorePorosity>(previousState) * timeStepWidth;
        ElasticStrain =
            previousState.ElasticStrain
            + (
                stepVector.StepVectorMap.HasItem<PoreElasticStrain>(previousState)
                    ? stepVector.ItemValue<PoreElasticStrain>(previousState) * timeStepWidth
                    : 0
            );
        PoreMaterial = previousState.PoreMaterial;
        Volume = this.Volume<Pore, NodeBase>();
        DenseVolume =
            previousState.DenseVolume
            + stepVector.ItemValue<PoreDenseVolume>(previousState) * timeStepWidth;
    }

    public Guid Id { get; }
    public IReadOnlyVertexCollection<NodeBase> Nodes { get; }
    public double Volume { get; }
    public double DenseVolume { get; }
    public double Porosity { get; }
    public double ElasticStrain { get; }

    public double PorousCompressionModulus =>
        4.0 / 3.0 * PoreMaterial.ViscoElastic.ElasticModulus * Pow(1 - Porosity, 3) / Porosity;

    public double PorousVolumeViscosity =>
        4.0 / 3.0 * PoreMaterial.ViscoElastic.ShearViscosity * Pow(1 - Porosity, 3) / Porosity;

    public IPoreMaterial PoreMaterial { get; }

    public double SpecificSurfaceArea => 2 * PI * PoreMaterial.AverageParticleRadius * Porosity;
    public double SpecificGrainBoundaryArea => 0;

    public double SpecificSurfaceAreaDerivative => 2 * PI * PoreMaterial.AverageParticleRadius;
    public double SpecificGrainBoundaryAreaDerivative => 0;

    public double ParticleCount => DenseVolume / (PI * Pow(PoreMaterial.AverageParticleRadius, 2));
    public double ParticleCountDerivative => 1 / (PI * Pow(PoreMaterial.AverageParticleRadius, 2));

    public Pore ApplyTimeStep(
        SolutionState solutionState,
        StepVector stepVector,
        double timeStepWidth
    ) => new(solutionState, this, stepVector, timeStepWidth);
}
