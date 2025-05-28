using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Pores;

public interface IPoreState<out TNode> : IPore<TNode>, IPoreDensity, IPorePressure
    where TNode : INode { }
