using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

public class Pore : IPore<NodeBase>, IPoreDensity, IPorePressure
{
    public Pore(Guid id, IEnumerable<NodeBase> nodes, double density, double pressure)
    {
        Id = id;
        Nodes = nodes.ToReadOnlyVertexCollection();
        RelativeDensity = density;
        Pressure = pressure;
        Volume = this.Volume<Pore, NodeBase>();
    }

    private Pore(
        Pore previousState,
        IReadOnlyVertexCollection<NodeBase> allNewNodes,
        StepVector stepVector,
        double timeStepWidth
    )
    {
        Id = previousState.Id;
        Nodes = previousState.Nodes.Select(n => allNewNodes[n.Id]).ToReadOnlyVertexCollection();
        RelativeDensity =
            previousState.RelativeDensity
            + stepVector.ItemValue<MatrixPoreDensity>(previousState) * timeStepWidth;
        Pressure =
            previousState.Pressure
            + stepVector.ItemValue<MatrixPorePressure>(previousState) * timeStepWidth;
    }

    public Guid Id { get; }
    public IReadOnlyVertexCollection<NodeBase> Nodes { get; }
    public double Volume { get; }
    public double RelativeDensity { get; }
    public double Pressure { get; }

    public Pore ApplyTimeStep(
        IReadOnlyVertexCollection<NodeBase> allNewNodes,
        StepVector stepVector,
        double timeStepWidth
    ) => new(this, allNewNodes, stepVector, timeStepWidth);
}
