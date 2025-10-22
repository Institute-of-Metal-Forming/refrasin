using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.Vertex;

namespace RefraSin.ParticleModel.Pores;

public class Pore<TNode> : IPore<TNode>
    where TNode : INode
{
    public Pore(Guid id, IEnumerable<TNode> nodes)
    {
        Id = id;
        Nodes = nodes.ToReadOnlyVertexCollection();
        Volume = this.Volume<Pore<TNode>, TNode>();
    }

    public Guid Id { get; }
    public IReadOnlyVertexCollection<TNode> Nodes { get; }

    public double Volume { get; }
}
