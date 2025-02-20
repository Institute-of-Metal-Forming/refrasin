using System.Runtime.CompilerServices;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public interface IQuantity
{
    public double DrivingForce(StepVector stepVector);

    public double Derivative(StepVector stepVector);
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

public abstract class QuantityBase(
    SolutionState solutionState,
    StepVector stepVector,
    IEnumerable<IConstraint> constraints
) : IQuantity
{
    public abstract double DrivingForce(StepVector stepVector);
    public abstract double Derivative(StepVector stepVector);

    protected readonly SolutionState SolutionState = solutionState;
    protected readonly StepVector StepVector = stepVector;
    protected readonly IReadOnlyList<IConstraint> Constraints = constraints.ToArray();
}
