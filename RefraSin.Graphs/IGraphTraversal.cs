namespace RefraSin.Graphs;

public interface IGraphTraversal<TVertex, TEdge>
    where TEdge : IEdge<TVertex>
    where TVertex : IVertex
{
    TVertex Start { get; }

    IEnumerable<TEdge> TraversedEdges { get; }
}
