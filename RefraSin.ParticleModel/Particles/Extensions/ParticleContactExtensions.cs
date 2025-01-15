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
        where TNode : IParticleNode
    {
        var possibleContactedNodes = self.To.ContactNodes<TParticle, TNode>().ToArray();

        return self.From.Nodes.Where(n =>
        {
            if (n.Type is NodeType.Surface)
                return false;

            if (n is INodeContact nodeContact)
                return nodeContact.ContactedParticleId == self.To.Id;

            return n.TryFindContactedNodeByCoordinates(possibleContactedNodes) is not null;
        });
    }

    public static IEnumerable<TNode> ToNodes<TParticle, TNode>(this IEdge<TParticle> self)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        var possibleContactedNodes = self.From.ContactNodes<TParticle, TNode>().ToArray();

        return self.To.Nodes.Where(n =>
        {
            if (n.Type is NodeType.Surface)
                return false;

            if (n is INodeContact nodeContact)
                return nodeContact.ContactedParticleId == self.From.Id;

            return n.TryFindContactedNodeByCoordinates(possibleContactedNodes) is not null;
        });
    }

    public static IEnumerable<IEdge<TNode>> NodePairs<TParticle, TNode>(this IEdge<TParticle> self)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        var possibleContactedNodes = self.To.ContactNodes<TParticle, TNode>().ToArray();

        foreach (var fromNode in self.From.Nodes)
        {
            if (fromNode.Type is NodeType.Surface)
                continue;

            if (fromNode is INodeContact nodeContact)
                yield return new Edge<TNode>(fromNode, self.To.Nodes[nodeContact.ContactedNodeId]);

            var foundByCoordinate = fromNode.TryFindContactedNodeByCoordinates(
                possibleContactedNodes
            );

            if (foundByCoordinate is not null)
                yield return new Edge<TNode>(fromNode, foundByCoordinate);
        }
    }

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
