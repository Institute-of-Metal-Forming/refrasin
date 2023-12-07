namespace RefraSin.ParticleModel;

public static class NodeCollectionExtensions
{
    public static ReadOnlyNodeCollection<TNode> ToReadOnlyNodeCollection<TNode>(this IEnumerable<TNode> source) where TNode : INode => new(source);

    public static Dictionary<Guid, TNode> ToDictionaryById<TNode>(this IEnumerable<TNode> source) where TNode : INode =>
        source.ToDictionary(n => n.Id, n => n);
}