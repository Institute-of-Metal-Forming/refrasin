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
}
