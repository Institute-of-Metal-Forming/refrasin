namespace RefraSin.ParticleModel;

public static class NodeCollectionExtensions
{
    public static NodeCollection<TNode> ToNodeCollection<TNode>(this IEnumerable<TNode> source) where TNode : INode => new(source);
}