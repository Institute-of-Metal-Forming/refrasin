namespace RefraSin.Graphs;

public class DepthFirstPathFinder<TVertex> : IGraphSearch<TVertex> where TVertex : IVertex
{
    private readonly DirectedEdge<TVertex>[] _exploredEdges;

    public DepthFirstPathFinder(IGraph<TVertex, IEdge<TVertex>> graph, TVertex start, TVertex target)
    {
        Start = start;
        Target = target;
        _exploredEdges = Explore(graph);
    }

    public DepthFirstPathFinder(IRootedGraph<TVertex, IEdge<TVertex>> graph, TVertex target)
    {
        Start = graph.Root;
        Target = target;
        _exploredEdges = Explore(graph);
    }

    private DirectedEdge<TVertex>[] Explore(IGraph<TVertex, IEdge<TVertex>> graph)
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { Start };

        IEnumerable<DirectedEdge<TVertex>>? InspectVertex(TVertex vertex)
        {
            foreach (var child in graph.ChildrenOf(vertex))
            {
                if (verticesVisited.Contains(child))
                    continue;

                if (child.Equals(Target))
                    return new[] { new DirectedEdge<TVertex>(vertex, child) };

                var childResult = InspectVertex(child);

                if (childResult is not null)
                    return childResult.Prepend(new DirectedEdge<TVertex>(vertex, child));
            }

            return null;
        }

        var result = InspectVertex(Start);

        if (result is null)
            throw new Exception("Target vertex is not reachable from start vertex.");

        return result.ToArray();
    }

    /// <inheritdoc />
    public TVertex Start { get; }

    public TVertex Target { get; }

    /// <inheritdoc />
    public IEnumerable<DirectedEdge<TVertex>> ExploredEdges => _exploredEdges;
}