namespace RefraSin.ParticleModel;

public static class NodeCollectionExtensions
{
    public static ReadOnlyNodeCollection<TNode> ToReadOnlyNodeCollection<TNode>(this IEnumerable<TNode> source) where TNode : INode => new(source);
}