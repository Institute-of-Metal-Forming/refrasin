using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class NormalDisplacement : IStateVelocity, INodeItem
{
    private NormalDisplacement(NodeBase node)
    {
        Node = node;
    }

    public static INodeItem Create(NodeBase node) => new NormalDisplacement(node);

    public double DrivingForce(StepVector stepVector) => -Node.GibbsEnergyGradient.Normal;

    public NodeBase Node { get; }

    public override string ToString() => $"normal displacement of {Node}";
}
