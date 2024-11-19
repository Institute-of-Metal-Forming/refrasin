namespace RefraSin.Graphs;

public class BreadthFirstExplorer<TVertex, TEdge> : IGraphTraversal<TVertex, TEdge>
    where TVertex : IVertex
    where TEdge : IEdge<TVertex>
{
    private readonly TEdge[] _exploredEdges;

    private BreadthFirstExplorer(TVertex start, TEdge[] exploredEdges)
    {
        Start = start;
        _exploredEdges = exploredEdges;
    }

    public static BreadthFirstExplorer<TVertex, TEdge> Explore(
        IGraph<TVertex, TEdge> graph,
        TVertex start
    ) => new(start, DoExplore(graph, start).ToArray());

    public static BreadthFirstExplorer<TVertex, TEdge> Explore(
        IRootedGraph<TVertex, TEdge> graph
    ) => new(graph.Root, DoExplore(graph, graph.Root).ToArray());

    private static IEnumerable<TEdge> DoExplore(IGraph<TVertex, TEdge> graph, TVertex start)
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { start };

        var queue = new Queue<TVertex>();
        queue.Enqueue(start);

        while (queue.TryDequeue(out var current))
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
                queue.Enqueue(child);
            }
        }
    }

    /// <inheritdoc />
    public TVertex Start { get; }

    /// <inheritdoc />
    public IEnumerable<TEdge> TraversedEdges => _exploredEdges;
}
