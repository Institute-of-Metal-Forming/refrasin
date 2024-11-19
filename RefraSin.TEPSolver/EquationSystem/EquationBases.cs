using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public abstract class EquationBase(StepVector step) : IEquation
{
    protected readonly StepVector Step = step;

    public StepVectorMap Map => Step.StepVectorMap;

    /// <inheritdoc />
    public abstract double Value();

    /// <inheritdoc />
    public abstract IEnumerable<(int, double)> Derivative();
}

public abstract class GlobalEquationBase(SolutionState state, StepVector step) : EquationBase(step)
{
    protected readonly SolutionState State = state;
}

public abstract class ParticleEquationBase(Particle particle, StepVector step) : EquationBase(step)
{
    protected readonly Particle Particle = particle;
}

public abstract class ContactEquationBase(ParticleContact contact, StepVector step)
    : EquationBase(step)
{
    protected readonly ParticleContact Contact = contact;
}

public abstract class RingEquationBase(ParticleRing ring, StepVector step) : EquationBase(step)
{
    protected readonly ParticleRing Ring = ring;
}

public abstract class NodeEquationBase<TNode>(TNode node, StepVector step) : EquationBase(step)
    where TNode : NodeBase
{
    protected readonly TNode Node = node;
    protected Particle Particle => Node.Particle;
}
