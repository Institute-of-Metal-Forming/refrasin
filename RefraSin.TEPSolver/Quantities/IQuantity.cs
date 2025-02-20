using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Quantities;

public interface IQuantity
{
    public double DrivingForce(StepVector stepVector);
}

public interface IGlobalQuantity : IQuantity
{
    static abstract IGlobalQuantity Create(SolutionState solutionState);
}

public interface IParticleQuantity : IQuantity
{
    static abstract IParticleQuantity Create(SolutionState solutionState, Particle particle);

    Particle Particle { get; }
}

public interface INodeQuantity : IQuantity
{
    static abstract INodeQuantity Create(SolutionState solutionState, NodeBase node);

    NodeBase Node { get; }
}

public interface INodeQuantity<out TNode> : INodeQuantity
    where TNode : NodeBase
{
    new TNode Node { get; }
}
