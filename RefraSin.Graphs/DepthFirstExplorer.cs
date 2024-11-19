namespace RefraSin.Graphs;

public class DepthFirstExplorer<TVertex, TEdge> : IGraphTraversal<TVertex, TEdge>
    where TEdge : IEdge<TVertex>
    where TVertex : IVertex
{
    private readonly TEdge[] _exploredEdges;

    private DepthFirstExplorer(TVertex start, TEdge[] exploredEdges)
    {
        Start = start;
        _exploredEdges = exploredEdges;
    }

    public static DepthFirstExplorer<TVertex, TEdge> Explore(
        IGraph<TVertex, TEdge> graph,
        TVertex start
    ) => new(start, DoExplore(graph, start).ToArray());

    public static DepthFirstExplorer<TVertex, TEdge> Explore(IRootedGraph<TVertex, TEdge> graph) =>
        new(graph.Root, DoExplore(graph, graph.Root).ToArray());

    private static IEnumerable<TEdge> DoExplore(IGraph<TVertex, TEdge> graph, TVertex start)
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { start };

        IEnumerable<TEdge> InspectVertex(TVertex current)
        {
            foreach (var edge in graph.EdgesFrom(current))
            {
                var child = edge.To;

                if (verticesVisited.Contains(child))
                {
                    yield return edge;
                    continue;
                }

                yield return edge;

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
    public IEnumerable<TEdge> TraversedEdges => _exploredEdges;
}
