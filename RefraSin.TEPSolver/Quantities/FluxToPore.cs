using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class FluxToPore(NodeBase node) : IFlux, INodeItem
{
    public double DissipationFactor(StepVector stepVector) =>
        Node.Particle.VacancyVolumeEnergy
        * 2 // half of surface distance
        / (
            Node.SurfaceDistance.ToUpper * Node.InterfaceTransferCoefficient.ToUpper
            + Node.SurfaceDistance.ToLower * Node.InterfaceTransferCoefficient.ToLower
        );

    public NodeBase Node { get; } = node;

    public override string ToString() => $"flux to pore from {Node}";
}
