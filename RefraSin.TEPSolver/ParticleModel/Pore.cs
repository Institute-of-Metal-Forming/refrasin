using RefraSin.MaterialData;
using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;
using RefraSin.Vertex;

namespace RefraSin.TEPSolver.ParticleModel;

public class Pore : IPore<NodeBase>, IPoreDensity, IPorePressure
{
    public Pore(
        IPore<INode> pore,
        SolutionState solutionState,
        double density,
        double pressure,
        IPoreMaterial poreMaterial
    )
    {
        Id = pore.Id;
        Nodes = pore.Nodes.Select(n => solutionState.Nodes[n.Id]).ToReadOnlyVertexCollection();
        Density = density;
        Pressure = pressure;
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
        Density =
            previousState.Density
            + stepVector.ItemValue<PoreDensity>(previousState) * timeStepWidth;
        Pressure =
            previousState.Pressure
            + stepVector.ItemValue<PorePressure>(previousState) * timeStepWidth;
        PoreMaterial = previousState.PoreMaterial;
    }

    public Guid Id { get; }
    public IReadOnlyVertexCollection<NodeBase> Nodes { get; }
    public double Volume { get; }
    public double Density { get; }
    public double Pressure { get; }

    public IPoreMaterial PoreMaterial { get; }

    public Pore ApplyTimeStep(
        SolutionState solutionState,
        StepVector stepVector,
        double timeStepWidth
    ) => new(solutionState, this, stepVector, timeStepWidth);
}
