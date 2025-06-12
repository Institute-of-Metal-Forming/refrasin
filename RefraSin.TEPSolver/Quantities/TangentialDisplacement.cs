using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class TangentialDisplacement(NodeBase node) : INodeItem, IStateVelocity
{
    public double DrivingForce(StepVector stepVector) => -Node.GibbsEnergyGradient.Tangential;

    public NodeBase Node { get; } = node;

    public override string ToString() => $"tangential displacement of {Node}";
}
