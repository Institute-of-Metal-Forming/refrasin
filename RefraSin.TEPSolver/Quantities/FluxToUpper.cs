using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class FluxToUpper : INodeQuantity, IFlux
{
    private FluxToUpper(NodeBase node)
    {
        Node = node;
    }

    public static INodeQuantity Create(NodeBase node) => new FluxToUpper(node);

    public double DissipationFactor(StepVector stepVector) =>
        Node.Particle.VacancyVolumeEnergy
        * Node.SurfaceDistance.ToUpper
        / Node.InterfaceDiffusionCoefficient.ToUpper;

    public NodeBase Node { get; }

    public override string ToString() => $"flux to upper from {Node}";
}
