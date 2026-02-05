using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.Vertex;

namespace RefraSin.ParticleModel.Pores;

public interface IPore : IVertex
{
    public double Volume { get; }
}

public interface IPore<out TNode> : IPore
    where TNode : INode
{
    public IReadOnlyVertexCollection<TNode> Nodes { get; }
}
