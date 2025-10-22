using MoreLinq;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.System;

namespace RefraSin.ParticleModel.Pores.Extensions;

public static class PoreDetectionExtensions
{
    public static IEnumerable<IPore<TNode>> DetectPores<TParticle, TNode>(
        this IParticleSystem<TParticle, TNode> system
    )
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode => system.Particles.DetectPores<TParticle, TNode>();

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

            yield return new Pore<TNode>(Guid.NewGuid(), FollowPoreNodes(allNodes, startingNode));
        }
    }

    public static IEnumerable<IPore<TNode>> UpdatePores<TPore, TNode>(
        this IEnumerable<TPore> pores,
        IEnumerable<TNode> allNodes
    )
        where TPore : IPore<TNode>
        where TNode : IParticleNode
    {
        var allNodesDict = allNodes
            .Where(n => n.Type is not NodeType.GrainBoundary)
            .ToDictionary(n => n.Id);

        foreach (var pore in pores)
        {
            TNode startingNode;
            var nodeEnumerator = pore.Nodes.GetEnumerator();
            while (true)
            {
                if (!nodeEnumerator.MoveNext())
                    throw new InvalidOperationException(
                        "None of the pores previous nodes is still present."
                    );

                if (allNodesDict.TryGetValue(nodeEnumerator.Current.Id, out var result))
                {
                    startingNode = result;
                    break;
                }
            }
            nodeEnumerator.Dispose();

            yield return new Pore<TNode>(pore.Id, FollowPoreNodes(allNodesDict, startingNode));
        }
    }

    static IEnumerable<TNode> FollowPoreNodes<TNode>(
        IDictionary<Guid, TNode> allNodes,
        TNode startingNode
    )
        where TNode : IParticleNode
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
                        currentNode = currentNode.FindContactedNodeByCoordinates(allNodes.Values);
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
