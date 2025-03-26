using RefraSin.ParticleModel;
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
    static abstract IParticleQuantity Create(Particle particle);

    Particle Particle { get; }
}

public interface INodeQuantity : IQuantity
{
    static abstract INodeQuantity Create(NodeBase node);

    NodeBase Node { get; }
}

public interface INodeContactQuantity : IQuantity
{
    static abstract INodeContactQuantity Create(ContactPair<NodeBase> nodeContact);

    ContactPair<NodeBase> NodeContact { get; }
}

public interface IParticleContactQuantity : IQuantity
{
    static abstract IParticleContactQuantity Create(ContactPair<Particle> nodeContact);

    ContactPair<Particle> ParticleContact { get; }
}
