using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles.Extensions;

public static class ParticleContactExtensions
{
    public static IEnumerable<TNode> FromNodes<TParticle, TNode>(
        this IParticleContactEdge<TParticle> self
    )
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        return self.From.Nodes.Where(n =>
        {
            if (n is INodeContact nodeContact)
                return nodeContact.ContactedParticleId == self.To.Id;
            return false;
        });
    }

    public static IEnumerable<TNode> ToNodes<TParticle, TNode>(
        this IParticleContactEdge<TParticle> self
    )
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        return self.To.Nodes.Where(n =>
        {
            if (n is INodeContact nodeContact)
                return nodeContact.ContactedParticleId == self.From.Id;
            return false;
        });
    }
}
