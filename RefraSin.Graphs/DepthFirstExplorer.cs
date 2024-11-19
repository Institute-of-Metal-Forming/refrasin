namespace RefraSin.Graphs;

public class DepthFirstExplorer<TVertex> : IGraphTraversal<TVertex>
    where TVertex : IVertex
{
    private readonly Edge<TVertex>[] _exploredEdges;

    private DepthFirstExplorer(TVertex start, Edge<TVertex>[] exploredEdges)
    {
        Start = start;
        _exploredEdges = exploredEdges;
    }

    public static DepthFirstExplorer<TVertex> Explore<TEdge>(
        IGraph<TVertex, TEdge> graph,
        TVertex start,
        bool allowBackstepping = true
    )
        where TEdge : IEdge<TVertex> =>
        new(start, DoExplore(graph, start, allowBackstepping).ToArray());

    public static DepthFirstExplorer<TVertex> Explore<TEdge>(
        IRootedGraph<TVertex, TEdge> graph,
        bool allowBackstepping = true
    )
        where TEdge : IEdge<TVertex> =>
        new(graph.Root, DoExplore(graph, graph.Root, allowBackstepping).ToArray());

    private static IEnumerable<Edge<TVertex>> DoExplore<TEdge>(
        IGraph<TVertex, TEdge> graph,
        TVertex start,
        bool allowBackstepping
    )
        where TEdge : IEdge<TVertex>
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { start };
        var edgesVisited = new HashSet<IEdge<TVertex>>(graph.EdgeCount);

        IEnumerable<Edge<TVertex>> InspectVertex(TVertex current)
        {
            foreach (var edge in graph.EdgesFrom(current))
            {
                if (!allowBackstepping && edgesVisited.Contains(edge.Reversed()))
                    continue;

                if (!edgesVisited.Add(edge))
                    continue;

                var child = edge.To;

                if (verticesVisited.Contains(child))
                {
                    yield return new Edge<TVertex>(current, child, true);
                    continue;
                }

                yield return new Edge<TVertex>(current, child, true);

                verticesVisited.Add(child);

                var childResult = InspectVertex(child);

                foreach (var r in childResult)
                    yield return r;
            }
        }

        var result = InspectVertex(start);

        if (result is null)
            throw new Exception("Target vertex is not reachable from start vertex.");

        return result.ToArray();
    }

    /// <inheritdoc />
    public TVertex Start { get; }

    /// <inheritdoc />
    public IEnumerable<IEdge<TVertex>> TraversedEdges => _exploredEdges;
}
