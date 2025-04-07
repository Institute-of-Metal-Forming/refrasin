using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class TangentialDisplacement : INodeQuantity, IStateVelocity
{
    private TangentialDisplacement(NodeBase node)
    {
        Node = node;
    }

    public static INodeQuantity Create(NodeBase node) => new TangentialDisplacement(node);

    public double DrivingForce(StepVector stepVector) => -Node.GibbsEnergyGradient.Tangential;

    public NodeBase Node { get; }
}
