namespace RefraSin.Graphs;

public class DepthFirstPathFinder<TVertex> : IGraphTraversal<TVertex> where TVertex : IVertex
{
    private readonly Edge<TVertex>[] _traversedEdges;


    private DepthFirstPathFinder(TVertex start, TVertex target, Edge<TVertex>[] traversedEdges)
    {
        Start = start;
        Target = target;
        _traversedEdges = traversedEdges;
    }

    public static DepthFirstPathFinder<TVertex> FindPath<TEdge>(IGraph<TVertex, TEdge> graph, TVertex start, TVertex target)
        where TEdge : IEdge<TVertex> =>
        new(
            start,
            target,
            DoFindPath(graph, start, target).ToArray()
        );

    public static DepthFirstPathFinder<TVertex> FindPath<TEdge>(IRootedGraph<TVertex, TEdge> graph, TVertex target) where TEdge : IEdge<TVertex> =>
        new(
            graph.Root,
            target,
            DoFindPath(graph, graph.Root, target).ToArray()
        );

    private static IEnumerable<Edge<TVertex>> DoFindPath<TEdge>(IGraph<TVertex, TEdge> graph, TVertex start, TVertex target) where TEdge : IEdge<TVertex>
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { start };

        IEnumerable<Edge<TVertex>>? InspectVertex(TVertex vertex)
        {
            foreach (var child in graph.ChildrenOf(vertex))
            {
                if (verticesVisited.Contains(child))
                    continue;

                verticesVisited.Add(child);

                if (child.Equals(target))
                    return new[] { new Edge<TVertex>(vertex, child, true) };

                var childResult = InspectVertex(child);

                if (childResult is not null)
                    return childResult.Prepend(new Edge<TVertex>(vertex, child, true));
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
    public IEnumerable<IEdge<TVertex>> TraversedEdges => _traversedEdges;
}