using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver;

public interface ISystemItem { }

public interface IGlobalItem : ISystemItem
{
    static abstract IGlobalItem Create(SolutionState solutionState);

    SolutionState SolutionState { get; }
}

public interface IParticleItem : ISystemItem
{
    static abstract IParticleItem Create(Particle particle);

    Particle Particle { get; }
}

public interface INodeItem : ISystemItem
{
    static abstract INodeItem Create(NodeBase node);

    NodeBase Node { get; }
}

public interface INodeContactItem : ISystemItem
{
    static abstract INodeContactItem Create(ContactPair<NodeBase> nodeContact);

    ContactPair<NodeBase> NodeContact { get; }
}

public interface IParticleContactItem : ISystemItem
{
    static abstract IParticleContactItem Create(ContactPair<Particle> nodeContact);

    ContactPair<Particle> ParticleContact { get; }
}
