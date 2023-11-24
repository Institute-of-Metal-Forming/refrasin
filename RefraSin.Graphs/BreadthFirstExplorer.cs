namespace RefraSin.Graphs;

public class BreadthFirstExplorer : IGraphSearch
{
    private readonly IEdge[] _exploredEdges;

    public BreadthFirstExplorer(IGraph graph, IVertex start)
    {
        Start = start;
        _exploredEdges = Explore(graph).ToArray();
    }

    public BreadthFirstExplorer(IRootedGraph graph)
    {
        Start = graph.Root;
        _exploredEdges = Explore(graph).ToArray();
    }

    private IEnumerable<IEdge> Explore(IGraph graph)
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { Start };

        var queue = new Queue<IVertex>();
        queue.Enqueue(Start);

        while (queue.TryDequeue(out var current))
        {
            foreach (var child in graph.ChildrenOf(current).Reverse())
            {
                yield return new DirectedEdge(current, child);

                if (verticesVisited.Contains(child))
                    continue;

                verticesVisited.Add(child);
                queue.Enqueue(child);
            }
        }
    }

    /// <inheritdoc />
    public IVertex Start { get; }

    /// <inheritdoc />
    public IEnumerable<IEdge> ExploredEdges => _exploredEdges;
}