namespace RefraSin.Graphs;

public class DepthFirstExplorer<TVertex> : IGraphSearch<TVertex> where TVertex : IVertex
{
    private readonly DirectedEdge<TVertex>[] _exploredEdges;

    private DepthFirstExplorer(TVertex start, DirectedEdge<TVertex>[] exploredEdges)
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

    private static IEnumerable<DirectedEdge<TVertex>> DoExplore<TEdge>(IGraph<TVertex, TEdge> graph, TVertex start) where TEdge : IEdge<TVertex>
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { start };

        IEnumerable<DirectedEdge<TVertex>> InspectVertex(TVertex vertex)
        {
            foreach (var child in graph.ChildrenOf(vertex))
            {
                yield return new DirectedEdge<TVertex>(vertex, child);

                if (verticesVisited.Contains(child))
                    continue;

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
    public IEnumerable<DirectedEdge<TVertex>> ExploredEdges => _exploredEdges;
}