using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public class NormalDisplacement(NodeBase node) : IStateVelocity, INodeItem
{
    public double DrivingForce(StepVector stepVector) => -Node.GibbsEnergyGradient.Normal;

    public NodeBase Node { get; } = node;

    public override string ToString() => $"normal displacement of {Node}";
}
