using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class FluxToUpper : INodeQuantity
{
    private FluxToUpper(NodeBase node)
    {
        Node = node;
    }

    public static INodeQuantity Create(NodeBase node) => new FluxToUpper(node);

    public double DrivingForce(StepVector stepVector) => 0;

    public NodeBase Node { get; }
}
