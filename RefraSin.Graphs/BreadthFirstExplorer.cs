namespace RefraSin.Graphs;

public class BreadthFirstExplorer<TVertex> : IGraphTraversal<TVertex> where TVertex : IVertex
{
    private readonly Edge<TVertex>[] _exploredEdges;

    private BreadthFirstExplorer(TVertex start, Edge<TVertex>[] exploredEdges)
    {
        Start = start;
        _exploredEdges = exploredEdges;
    }

    public static BreadthFirstExplorer<TVertex> Explore<TEdge>(IGraph<TVertex, TEdge> graph, TVertex start, bool allowBackstepping = true)
        where TEdge : IEdge<TVertex> =>
        new(
            start,
            DoExplore(graph, start, allowBackstepping).ToArray()
        );

    public static BreadthFirstExplorer<TVertex> Explore<TEdge>(IRootedGraph<TVertex, TEdge> graph, bool allowBackstepping = true)
        where TEdge : IEdge<TVertex> =>
        new(
            graph.Root,
            DoExplore(graph, graph.Root, allowBackstepping).ToArray()
        );

    private static IEnumerable<Edge<TVertex>> DoExplore<TEdge>(IGraph<TVertex, TEdge> graph, TVertex start, bool allowBackstepping)
        where TEdge : IEdge<TVertex>
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { start };
        var edgesVisited = new HashSet<IEdge<TVertex>>(graph.EdgeCount);

        var queue = new Queue<TVertex>();
        queue.Enqueue(start);

        while (queue.TryDequeue(out var current))
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
                queue.Enqueue(child);
            }
        }
    }

    /// <inheritdoc />
    public TVertex Start { get; }

    /// <inheritdoc />
    public IEnumerable<IEdge<TVertex>> TraversedEdges => _exploredEdges;
}