using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Pores;

public interface IPoreState<out TNode> : IPore<TNode>, IPorePorosity, IPorePressure
    where TNode : INode { }
