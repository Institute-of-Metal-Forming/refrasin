using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Nodes.Extensions;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.Particles.Extensions;

public static class ParticleContactExtensions
{
    public static IEnumerable<TNode> SelectContactNodesByType<TParticle, TNode>(this TParticle self)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode => self.Nodes.Where(n => n.Type is GrainBoundary or Neck);

    public static IEnumerable<UnorderedPair<TNode>> EnumerateContactNodePairs<TParticle, TNode>(
        this TParticle self,
        TParticle other
    )
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        var possibleContactedNodes = other
            .SelectContactNodesByType<TParticle, TNode>()
            .ToDictionary(n => n.Id);

        foreach (var node in self.SelectContactNodesByType<TParticle, TNode>())
        {
            if (node is INodeContact nodeContact)
            {
                if (
                    possibleContactedNodes.TryGetValue(
                        nodeContact.ContactedNodeId,
                        out var contactedNode
                    )
                )
                    yield return new UnorderedPair<TNode>(node, contactedNode);
            }
            else if (
                node.TryFindContactedNodeByCoordinates(
                    possibleContactedNodes.Values,
                    out var contactedNode
                )
            )
            {
                yield return new UnorderedPair<TNode>(node, contactedNode);
            }
        }
    }

    public static IEnumerable<UnorderedPair<TNode>> EnumerateContactNodePairs<TParticle, TNode>(
        this IEnumerable<TParticle> self
    )
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        var visitedParticles = new List<TParticle>();

        foreach (var particle in self)
        {
            foreach (var other in visitedParticles)
            {
                if (
                    particle
                        .ToMeasures<TParticle, TNode>()
                        .MayIntersectWithByRectangularApproximation(
                            other.ToMeasures<TParticle, TNode>()
                        )
                )
                {
                    foreach (
                        var pair in particle.EnumerateContactNodePairs<TParticle, TNode>(other)
                    )
                    {
                        yield return pair;
                    }
                }
            }

            visitedParticles.Add(particle);
        }
    }

    public static IEnumerable<UnorderedPair<TParticle>> EnumerateContactedParticlePairs<
        TParticle,
        TNode
    >(this IEnumerable<TParticle> self)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        var visitedParticles = new List<TParticle>();

        foreach (var particle in self)
        {
            foreach (var other in visitedParticles)
            {
                if (
                    particle
                        .ToMeasures<TParticle, TNode>()
                        .MayIntersectWithByRectangularApproximation(
                            other.ToMeasures<TParticle, TNode>()
                        )
                )
                {
                    foreach (var _ in particle.EnumerateContactNodePairs<TParticle, TNode>(other))
                    {
                        yield return new UnorderedPair<TParticle>(particle, other);
                        break;
                    }
                }
            }

            visitedParticles.Add(particle);
        }
    }

    public static IEnumerable<TNode> FirstNodes<TParticle, TNode>(
        this UnorderedPair<TParticle> self
    )
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode =>
        self.First.EnumerateContactNodePairs<TParticle, TNode>(self.Second).Select(p => p.First);

    public static IEnumerable<TNode> SecondNodes<TParticle, TNode>(
        this UnorderedPair<TParticle> self
    )
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode =>
        self.First.EnumerateContactNodePairs<TParticle, TNode>(self.Second).Select(p => p.Second);
}
