using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.Constraints;

public interface IConstraint
{
    double Residual(StepVector stepVector);

    IEnumerable<(int index, double value)> Derivatives(StepVector stepVector);

    IEnumerable<(int firstIndex, int secondIndex, double value)> SecondDerivatives(
        StepVector stepVector
    ) => [];
}

public interface IGlobalConstraint : IConstraint
{
    static abstract IGlobalConstraint Create(SolutionState solutionState);
}

public interface IParticleConstraint : IConstraint
{
    static abstract IParticleConstraint Create(Particle particle);

    Particle Particle { get; }
}

public interface INodeConstraint : IConstraint
{
    static abstract INodeConstraint Create(NodeBase node);

    NodeBase Node { get; }
}
