namespace RefraSin.Graphs;

public class DepthFirstExplorer<TVertex> : IGraphSearch<TVertex> where TVertex : IVertex
{
    private readonly DirectedEdge<TVertex>[] _exploredEdges;

    public DepthFirstExplorer(IGraph<TVertex, IEdge<TVertex>> graph, TVertex start)
    {
        Start = start;
        _exploredEdges = Explore(graph).ToArray();
    }

    public DepthFirstExplorer(IRootedGraph<TVertex, IEdge<TVertex>> graph)
    {
        Start = graph.Root;
        _exploredEdges = Explore(graph).ToArray();
    }

    private IEnumerable<DirectedEdge<TVertex>> Explore(IGraph<TVertex, IEdge<TVertex>> graph)
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { Start };

        var stack = new Stack<TVertex>();
        stack.Push(Start);

        while (stack.TryPop(out var current))
        {
            foreach (var child in graph.ChildrenOf(current).Reverse())
            {
                yield return new DirectedEdge<TVertex>(current, child);

                if (verticesVisited.Contains(child))
                    continue;

                verticesVisited.Add(child);
                stack.Push(child);
            }
        }
    }

    /// <inheritdoc />
    public TVertex Start { get; }

    /// <inheritdoc />
    public IEnumerable<DirectedEdge<TVertex>> ExploredEdges => _exploredEdges;
}