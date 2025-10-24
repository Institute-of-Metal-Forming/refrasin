using MoreLinq;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.System;
using RefraSin.Vertex;

namespace RefraSin.ParticleModel.Pores.Extensions;

public static class PoreDetectionExtensions
{
    public static IParticleSystemWithPores<TParticle, TNode, IPore<TNode>> DetectPores<
        TParticle,
        TNode
    >(this IParticleSystem<TParticle, TNode> system)
        where TParticle : IParticle<TNode>
        where TNode : IParticleNode =>
        new ParticleSystemWithPores<TParticle, TNode, IPore<TNode>>(
            system.Particles,
            system.Particles.DetectPores<TParticle, TNode>().ToReadOnlyVertexCollection()
        );

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
                if (currentNode.Lower.Type is NodeType.GrainBoundary)
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
                    currentNode = allNodes[currentNode.Lower.Id];
                }
            }
            else
            {
                currentNode = allNodes[currentNode.Lower.Id];
            }

            allNodes.Remove(currentNode.Id);
        } while (!currentNode.Equals(startingNode));
    }

    public static IEnumerable<TPore> WithoutOuterSurface<TPore, TNode>(
        this IEnumerable<TPore> pores
    )
        where TNode : IParticleNode
        where TPore : IPore<TNode> => pores.Where(p => p.Volume >= 0);

    public static IParticleSystemWithPores<TParticle, TNode, TPore> WithoutOuterSurface<
        TParticle,
        TNode,
        TPore
    >(this IParticleSystemWithPores<TParticle, TNode, TPore> system)
        where TNode : IParticleNode
        where TPore : IPore<TNode>
        where TParticle : IParticle<TNode> =>
        new ParticleSystemWithPores<TParticle, TNode, TPore>(
            system.Particles,
            system.Pores.WithoutOuterSurface<TPore, TNode>().ToReadOnlyVertexCollection()
        );
}
