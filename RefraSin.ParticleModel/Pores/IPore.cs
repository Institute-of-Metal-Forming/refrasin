using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.Vertex;

namespace RefraSin.ParticleModel.Pores;

public interface IPore<out TNode> : IVertex
    where TNode : INode
{
    public IReadOnlyVertexCollection<TNode> Nodes { get; }

    public double Volume { get; }
}
