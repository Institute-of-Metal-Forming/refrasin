namespace RefraSin.Graphs;

public class DepthFirstExplorer : IGraphSearch
{
    private readonly IEdge[] _exploredEdges;

    public DepthFirstExplorer(IGraph graph, IVertex start)
    {
        Start = start;
        _exploredEdges = Explore(graph).ToArray();
    }

    public DepthFirstExplorer(IRootedGraph graph)
    {
        Start = graph.Root;
        _exploredEdges = Explore(graph).ToArray();
    }

    private IEnumerable<IEdge> Explore(IGraph graph)
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { Start };

        var stack = new Stack<IVertex>();
        stack.Push(Start);

        while (stack.TryPop(out var current))
        {
            foreach (var child in graph.ChildrenOf(current).Reverse())
            {
                yield return new DirectedEdge(current, child);

                if (verticesVisited.Contains(child))
                    continue;

                verticesVisited.Add(child);
                stack.Push(child);
            }
        }
    }

    /// <inheritdoc />
    public IVertex Start { get; }

    /// <inheritdoc />
    public IEnumerable<IEdge> ExploredEdges => _exploredEdges;
}