namespace RefraSin.Graphs;

public class BreadthFirstExplorer<TVertex, TEdge> : IGraphTraversal<TVertex, TEdge>
    where TVertex : IVertex
    where TEdge : IEdge<TVertex>, IReversibleEdge<TEdge>
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
        var edgesVisited = new HashSet<TEdge>(graph.EdgeCount);

        var queue = new Queue<TVertex>();
        queue.Enqueue(start);

        while (queue.TryDequeue(out var current))
        {
            foreach (var edge in graph.EdgesFrom(current))
            {
                if (!edgesVisited.Add(edge))
                    continue;

                edgesVisited.Add(edge.Reversed());

                var child = edge.To;
                yield return edge;

                if (!verticesVisited.Add(child))
                    continue;

                queue.Enqueue(child);
            }
        }
    }

    /// <inheritdoc />
    public TVertex Start { get; }

    /// <inheritdoc />
    public IEnumerable<TEdge> TraversedEdges => _exploredEdges;
}
