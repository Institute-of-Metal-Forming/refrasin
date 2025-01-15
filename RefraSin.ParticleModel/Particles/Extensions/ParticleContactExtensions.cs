using RefraSin.Graphs;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Nodes.Extensions;

namespace RefraSin.ParticleModel.Particles.Extensions;

public static class ParticleContactExtensions
{
    public static IEnumerable<TNode> ContactNodes<TParticle, TNode>(this TParticle self)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode => self.Nodes.Where(n => n.Type is not NodeType.Surface);

    public static IEnumerable<TNode> FromNodes<TParticle, TNode>(this IEdge<TParticle> self)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode =>
        self.From.Nodes.Where(n =>
        {
            if (n.Type is NodeType.Surface)
                return false;

            if (n is INodeContact nodeContact)
                return nodeContact.ContactedParticleId == self.To.Id;

            return n.TryFindContactedNodeByCoordinates(self.To.ContactNodes<TParticle, TNode>())
                is not null;
        });

    public static IEnumerable<Guid> ContactedParticles<TParticle, TNode>(this TParticle self)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        var contactedParticles = new HashSet<Guid>();

        foreach (var n in self.ContactNodes<TParticle, TNode>())
        {
            if (n is INodeContact nodeContact)
            {
                contactedParticles.Add(nodeContact.ContactedParticleId);
                continue;
            }

            throw new InvalidOperationException("Contact nodes must implement INodeContact");
        }

        return contactedParticles;
    }

    public static IEnumerable<TParticle> ContactedParticles<TParticle, TNode>(
        this TParticle self,
        IEnumerable<TParticle> otherParticles
    )
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        var otherParticlesDictionary = otherParticles.ToDictionary(p => p.Id, p => p);
        var contactedParticles = new HashSet<Guid>();
        var allContactNodes = otherParticlesDictionary
            .Values.SelectMany(p => p.ContactNodes<TParticle, TNode>())
            .ToArray();

        foreach (var n in self.ContactNodes<TParticle, TNode>())
        {
            if (n is INodeContact nodeContact)
            {
                contactedParticles.Add(nodeContact.ContactedParticleId);
                continue;
            }

            contactedParticles.Add(n.FindContactedNodeByCoordinates(allContactNodes).ParticleId);
        }

        return contactedParticles.Select(i => otherParticlesDictionary[i]);
    }
}
