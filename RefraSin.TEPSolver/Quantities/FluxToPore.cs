using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.TEPSolver.Quantities;

public class FluxToPore(NodeBase node) : IFlux, INodeItem
{
    public double DissipationFactor(StepVector stepVector) =>
        Node.Particle.VacancyVolumeEnergy
        * 2 // half of surface distance
        / (
            (
                Node.Upper.Type is Surface // avoid influence of grain boundaries at neck nodes
                    ? Node.SurfaceDistance.ToUpper * Node.InterfaceTransferCoefficient.ToUpper
                    : 0
            )
            + (
                Node.Lower.Type is Surface // avoid influence of grain boundaries at neck nodes
                    ? Node.SurfaceDistance.ToLower * Node.InterfaceTransferCoefficient.ToLower
                    : 0
            )
        );

    public NodeBase Node { get; } = node;

    public override string ToString() => $"flux to pore from {Node}";
}
