using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public interface IConstraint
{
    public double Residual(StepVector stepVector);

    public double DerivativeFor(StepVector stepVector, IQuantity quantity);
}

public interface IGlobalConstraint : IConstraint
{
    static abstract IGlobalConstraint Create(SolutionState solutionState);
}

public interface IParticleConstraint : IConstraint
{
    static abstract IParticleConstraint Create(SolutionState solutionState, Particle particle);

    Particle Particle { get; }
}

public interface INodeConstraint : IConstraint
{
    static abstract INodeConstraint Create(SolutionState solutionState, NodeBase node);

    NodeBase Node { get; }
}

public abstract class ConstraintBase(SolutionState solutionState, StepVector stepVector)
    : IConstraint
{
    protected readonly SolutionState SolutionState = solutionState;
    protected readonly StepVector StepVector = stepVector;

    public abstract double Residual(StepVector stepVector);

    public abstract double DerivativeFor(StepVector stepVector, IQuantity quantity);
}
