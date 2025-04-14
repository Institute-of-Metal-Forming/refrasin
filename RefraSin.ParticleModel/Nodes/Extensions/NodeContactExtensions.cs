using System.Diagnostics.CodeAnalysis;
using static RefraSin.ParticleModel.Nodes.NodeType;

namespace RefraSin.ParticleModel.Nodes.Extensions;

public static class NodeContactExtensions
{
    public static TNode? TryFindContactedNodeByCoordinates<TNode>(
        this TNode self,
        IEnumerable<TNode> allContactNodes,
        double precision = 1e-4
    )
        where TNode : INode
    {
        var absolutePrecision = precision * self.Coordinates.R;

        var contactedNode = allContactNodes.FirstOrDefault(n =>
            n.Id != self.Id
            && n.Coordinates.Absolute.IsClose(self.Coordinates.Absolute, absolutePrecision)
        );

        return contactedNode;
    }

    public static bool TryFindContactedNodeByCoordinates<TNode>(
        this TNode self,
        IEnumerable<TNode> allContactNodes,
        [MaybeNullWhen(false)] out TNode contacted,
        double precision = 1e-4
    )
        where TNode : INode
    {
        contacted = self.TryFindContactedNodeByCoordinates(allContactNodes, precision);
        return contacted is not null;
    }

    public static TNode FindContactedNodeByCoordinates<TNode>(
        this TNode self,
        IEnumerable<TNode> allContactNodes,
        double precision = 1e-4
    )
        where TNode : INode
    {
        var contactedNode = TryFindContactedNodeByCoordinates(self, allContactNodes, precision);

        if (contactedNode is null)
            throw new InvalidOperationException(
                $"No corresponding node with same location as {self} could be found."
            );

        return contactedNode;
    }

    public static IEnumerable<ContactPair<TNode>> CreateContactNodePairs<TNode>(
        this IEnumerable<TNode> self
    )
        where TNode : INode
    {
        var visitedNodes = new Dictionary<Guid, TNode>();

        foreach (var node in self)
        {
            if (node.Type is GrainBoundary or Neck)
            {
                if (node is INodeContact nodeContact)
                {
                    if (visitedNodes.TryGetValue(nodeContact.ContactedNodeId, out var other))
                        yield return new ContactPair<TNode>(Guid.NewGuid(), node, other);
                }
                else if (node.TryFindContactedNodeByCoordinates(visitedNodes.Values, out var other))
                {
                    yield return new ContactPair<TNode>(Guid.NewGuid(), node, other);
                }

                visitedNodes.Add(node.Id, node);
            }
        }
    }
}
