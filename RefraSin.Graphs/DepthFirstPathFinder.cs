namespace RefraSin.Graphs;

public class DepthFirstPathFinder<TVertex, TEdge> : IGraphTraversal<TVertex, TEdge>
    where TEdge : IEdge<TVertex>
    where TVertex : IVertex
{
    private readonly TEdge[] _traversedEdges;

    private DepthFirstPathFinder(TVertex start, TVertex target, TEdge[] traversedEdges)
    {
        Start = start;
        Target = target;
        _traversedEdges = traversedEdges;
    }

    public static DepthFirstPathFinder<TVertex, TEdge> FindPath(
        IGraph<TVertex, TEdge> graph,
        TVertex start,
        TVertex target
    ) => new(start, target, DoFindPath(graph, start, target).ToArray());

    public static DepthFirstPathFinder<TVertex, TEdge> FindPath(
        IRootedGraph<TVertex, TEdge> graph,
        TVertex target
    ) => new(graph.Root, target, DoFindPath(graph, graph.Root, target).ToArray());

    private static IEnumerable<TEdge> DoFindPath(
        IGraph<TVertex, TEdge> graph,
        TVertex start,
        TVertex target
    )
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { start };

        IEnumerable<TEdge>? InspectVertex(TVertex vertex)
        {
            foreach (var edge in graph.EdgesFrom(vertex))
            {
                var child = edge.To;
                if (verticesVisited.Contains(child))
                    continue;

                verticesVisited.Add(child);

                if (child.Equals(target))
                    return new[] { edge };

                var childResult = InspectVertex(child);

                if (childResult is not null)
                    return childResult.Prepend(edge);
            }

            return null;
        }

        var result = InspectVertex(start);

        if (result is null)
            throw new Exception("Target vertex is not reachable from start vertex.");

        return result.ToArray();
    }

    /// <inheritdoc />
    public TVertex Start { get; }

    public TVertex Target { get; }

    /// <inheritdoc />
    public IEnumerable<TEdge> TraversedEdges => _traversedEdges;
}
