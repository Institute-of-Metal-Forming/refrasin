using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver;

public interface ISystemItem { }

public interface IGlobalItem : ISystemItem
{
    SolutionState SolutionState { get; }
}

public interface IParticleItem : ISystemItem
{
    Particle Particle { get; }
}

public interface INodeItem : ISystemItem
{
    NodeBase Node { get; }
}

public interface IPoreItem : ISystemItem
{
    Pore Pore { get; }
}

public interface INodeContactItem : ISystemItem
{
    ContactPair<NodeBase> NodeContact { get; }
}

public interface IParticleContactItem : ISystemItem
{
    ContactPair<Particle> ParticleContact { get; }
}
