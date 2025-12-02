using RefraSin.MaterialData;
using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
using RefraSin.Vertex;

namespace RefraSin.TEPSolver.ParticleModel;

public class Pore : IPore<NodeBase>, IPorePorosity, IPorePressure
{
    public Pore(
        IPore<INode> pore,
        SolutionState solutionState,
        double porosity,
        double hydrostaticStress,
        IPoreMaterial poreMaterial
    )
    {
        Id = pore.Id;
        Nodes = pore.Nodes.Select(n => solutionState.Nodes[n.Id]).ToReadOnlyVertexCollection();
        Porosity = porosity;
        HydrostaticStress = hydrostaticStress;
        Volume = this.Volume<Pore, NodeBase>();
        PoreMaterial = poreMaterial;
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
        HydrostaticStress =
            previousState.HydrostaticStress
            + (
                stepVector.StepVectorMap.HasItem<PoreHydrostaticStress>(previousState)
                    ? stepVector.ItemValue<PoreHydrostaticStress>(previousState) * timeStepWidth
                    : 0
            );
        PoreMaterial = previousState.PoreMaterial;
        Volume = this.Volume<Pore, NodeBase>();
    }

    public Guid Id { get; }
    public IReadOnlyVertexCollection<NodeBase> Nodes { get; }
    public double Volume { get; }
    public double Porosity { get; }
    public double HydrostaticStress { get; }

    public double PorousCompressionModulus
    {
        get
        {
            var elasticModulus =
                PoreMaterial.ViscoElastic.ElasticModulus * (1 - 5.0 / 3.0 * Porosity);
            return 4.0 / 3.0 * elasticModulus * (1 - Porosity) / Porosity;
        }
    }

    public double PorousVolumeViscosity =>
        4.0 / 3.0 * PoreMaterial.ViscoElastic.ShearViscosity * Pow(1 - Porosity, 3) / Porosity;

    public IPoreMaterial PoreMaterial { get; }

    public Pore ApplyTimeStep(
        SolutionState solutionState,
        StepVector stepVector,
        double timeStepWidth
    ) => new(solutionState, this, stepVector, timeStepWidth);
}
