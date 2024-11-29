using RefraSin.ParticleModel.System;

namespace RefraSin.ProcessModel;

/// <summary>
/// Interface for classes representing the state of a particle group at a certain position in time and process line.
/// </summary>
public interface ISystemState<out TParticle, out TNode> : IParticleSystem<TParticle, TNode>, ISystemState
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
{
    
}

public interface ISystemState
{
    /// <summary>
    /// Unique ID of this state.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Time coordinate.
    /// </summary>
    double Time { get; }
}
