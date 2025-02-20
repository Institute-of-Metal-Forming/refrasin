using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class NormalDisplacement : INodeQuantity
{
    private NormalDisplacement(NodeBase node)
    {
        Node = node;
    }

    public static INodeQuantity Create(SolutionState solutionState, NodeBase node) =>
        new NormalDisplacement(node);

    public double DrivingForce(StepVector stepVector) => -Node.GibbsEnergyGradient.Normal;

    public NodeBase Node { get; }
}
