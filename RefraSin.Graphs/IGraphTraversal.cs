namespace RefraSin.Graphs;

public interface IGraphTraversal<TVertex>
    where TVertex : IVertex
{
    TVertex Start { get; }

    IEnumerable<IEdge<TVertex>> TraversedEdges { get; }
}
