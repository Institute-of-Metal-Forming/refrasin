using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class FluxToUpper(NodeBase node) : IFlux, INodeItem
{
    public double DissipationFactor(StepVector stepVector) =>
        Node.Particle.VacancyVolumeEnergy
        * Node.SurfaceDistance.ToUpper
        / Node.InterfaceDiffusionCoefficient.ToUpper;

    public NodeBase Node { get; } = node;

    public override string ToString() => $"flux to upper from {Node}";
}
