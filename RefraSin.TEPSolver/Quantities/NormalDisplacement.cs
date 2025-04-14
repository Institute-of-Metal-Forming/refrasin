using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class NormalDisplacement : INodeQuantity, IStateVelocity
{
    private NormalDisplacement(NodeBase node)
    {
        Node = node;
    }

    public static INodeQuantity Create(NodeBase node) => new NormalDisplacement(node);

    public double DrivingForce(StepVector stepVector) => -Node.GibbsEnergyGradient.Normal;

    public NodeBase Node { get; }

    public override string ToString() => $"normal displacement of {Node}";
}
