using MoreLinq;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.Pores.Extensions;

public static class PoreDetectionExtensions
{
    public static IEnumerable<IPore<TNode>> DetectPores<TParticle, TNode>(
        this IEnumerable<TParticle> particles
    )
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode
    {
        var allNodes = particles
            .SelectMany(p => p.Nodes.Where(n => n.Type is not NodeType.GrainBoundary))
            .ToDictionary(n => n.Id);

        while (allNodes.Count > 0)
        {
            var startingNode = allNodes.Values.First();

            yield return new Pore<TNode>(Guid.NewGuid(), FollowPoreNodes(startingNode));
        }

        IEnumerable<TNode> FollowPoreNodes(TNode startingNode)
        {
            var currentNode = startingNode;

            do
            {
                yield return currentNode;

                if (currentNode.Type is NodeType.Neck)
                {
                    if (currentNode.Upper.Type is NodeType.GrainBoundary)
                    {
                        if (currentNode is INodeContact contact)
                            currentNode = allNodes[contact.ContactedNodeId];
                        else
                        {
                            currentNode = currentNode.FindContactedNodeByCoordinates(
                                allNodes.Values
                            );
                        }
                    }
                    else
                    {
                        currentNode = allNodes[currentNode.Upper.Id];
                    }
                }
                else
                {
                    currentNode = allNodes[currentNode.Upper.Id];
                }

                allNodes.Remove(currentNode.Id);
            } while (!currentNode.Equals(startingNode));
        }
    }

    public static IEnumerable<TPore> WithoutOuterSurface<TPore, TNode>(
        this IEnumerable<TPore> pores
    )
        where TNode : IParticleNode
        where TPore : IPore<TNode>
    {
        var poresDictionary = pores.ToDictionary(p => p.Id);
        var largestPore = poresDictionary.Values.MaxBy(p => p.Volume);
        poresDictionary.Remove(
            largestPore?.Id ?? throw new InvalidOperationException("pore sequence empty")
        );
        return poresDictionary.Values;
    }
}
