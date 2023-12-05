namespace RefraSin.Graphs;

public class DepthFirstExplorer<TVertex> : IGraphTraversal<TVertex> where TVertex : IVertex
{
    private readonly TraversedEdge<TVertex>[] _exploredEdges;

    private DepthFirstExplorer(TVertex start, TraversedEdge<TVertex>[] exploredEdges)
    {
        Start = start;
        _exploredEdges = exploredEdges;
    }

    public static DepthFirstExplorer<TVertex> Explore<TEdge>(IGraph<TVertex, TEdge> graph, TVertex start) where TEdge : IEdge<TVertex> =>
        new(
            start,
            DoExplore(graph, start).ToArray()
        );

    public static DepthFirstExplorer<TVertex> Explore<TEdge>(IRootedGraph<TVertex, TEdge> graph) where TEdge : IEdge<TVertex> =>
        new(
            graph.Root,
            DoExplore(graph, graph.Root).ToArray()
        );

    private static IEnumerable<TraversedEdge<TVertex>> DoExplore<TEdge>(IGraph<TVertex, TEdge> graph, TVertex start) where TEdge : IEdge<TVertex>
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { start };
        var edgesVisited = new HashSet<TEdge>(graph.EdgeCount);

        IEnumerable<TraversedEdge<TVertex>> InspectVertex(TVertex current)
        {
            foreach (var edge in graph.EdgesFrom(current))
            {
                if (edgesVisited.Contains(edge))
                    continue;

                edgesVisited.Add(edge);

                var child = edge.Start.Equals(current) ? edge.End : edge.Start;

                if (verticesVisited.Contains(child))
                {
                    yield return new TraversedEdge<TVertex>(current, child, true);
                    continue;
                }

                yield return new TraversedEdge<TVertex>(current, child, false);

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
    public IEnumerable<TraversedEdge<TVertex>> TraversedEdges => _exploredEdges;
}