namespace RefraSin.Graphs;

public class BreadthFirstExplorer<TVertex> : IGraphSearch<TVertex> where TVertex : IVertex
{
    private readonly DirectedEdge<TVertex>[] _exploredEdges;

    public BreadthFirstExplorer(IGraph<TVertex, IEdge<TVertex>> graph, TVertex start)
    {
        Start = start;
        _exploredEdges = Explore(graph).ToArray();
    }

    public BreadthFirstExplorer(IRootedGraph<TVertex, IEdge<TVertex>> graph)
    {
        Start = graph.Root;
        _exploredEdges = Explore(graph).ToArray();
    }

    private IEnumerable<DirectedEdge<TVertex>> Explore(IGraph<TVertex, IEdge<TVertex>> graph)
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { Start };

        var queue = new Queue<TVertex>();
        queue.Enqueue(Start);

        while (queue.TryDequeue(out var current))
        {
            foreach (var child in graph.ChildrenOf(current).Reverse())
            {
                yield return new DirectedEdge<TVertex>(current, child);

                if (verticesVisited.Contains(child))
                    continue;

                verticesVisited.Add(child);
                queue.Enqueue(child);
            }
        }
    }

    /// <inheritdoc />
    public TVertex Start { get; }

    /// <inheritdoc />
    public IEnumerable<DirectedEdge<TVertex>> ExploredEdges => _exploredEdges;

}