using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.System;

namespace RefraSin.ProcessModel;

public interface ISystemStateWithPores<out TParticle, out TNode, out TPore>
    : ISystemState<TParticle, TNode>,
        IParticleSystemWithPores<TParticle, TNode, TPore>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
    where TPore : IPoreState<TNode> { }
