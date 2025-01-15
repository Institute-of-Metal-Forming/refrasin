using RefraSin.Graphs;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public abstract class EquationBase(SolutionState state, StepVector step) : IEquation
{
    protected readonly StepVector Step = step;

    protected readonly SolutionState State = state;

    public StepVectorMap Map => Step.StepVectorMap;

    /// <inheritdoc />
    public abstract double Value();

    /// <inheritdoc />
    public abstract IEnumerable<(int, double)> Derivative();
}

public abstract class ParticleEquationBase(SolutionState state, Particle particle, StepVector step)
    : EquationBase(state, step)
{
    protected readonly Particle Particle = particle;
}

public abstract class NodeEquationBase<TNode>(SolutionState state, TNode node, StepVector step)
    : EquationBase(state, step)
    where TNode : NodeBase
{
    protected readonly TNode Node = node;
    protected Particle Particle => Node.Particle;
}
