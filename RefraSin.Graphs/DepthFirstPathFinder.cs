namespace RefraSin.Graphs;

public class DepthFirstPathFinder<TVertex> : IGraphSearch<TVertex> where TVertex : IVertex
{
    private readonly DirectedEdge<TVertex>[] _exploredEdges;


    private DepthFirstPathFinder(TVertex start, TVertex target, DirectedEdge<TVertex>[] exploredEdges)
    {
        Start = start;
        Target = target;
        _exploredEdges = exploredEdges;
    }

    public static DepthFirstPathFinder<TVertex> FindPath<TEdge>(IGraph<TVertex, TEdge> graph, TVertex start, TVertex target)
        where TEdge : IEdge<TVertex> =>
        new(
            start,
            target,
            DoFindPath(graph, start, target).ToArray()
        );

    public static DepthFirstPathFinder<TVertex> FindPath<TEdge>(IRootedGraph<TVertex, TEdge> graph, TVertex target) where TEdge : IEdge<TVertex> =>
        new(
            graph.Root,
            target,
            DoFindPath(graph, graph.Root, target).ToArray()
        );

    private static IEnumerable<DirectedEdge<TVertex>> DoFindPath<TEdge>(IGraph<TVertex, TEdge> graph, TVertex start, TVertex target) where TEdge : IEdge<TVertex>
    {
        var verticesVisited = new HashSet<IVertex>(graph.VertexCount) { start };

        IEnumerable<DirectedEdge<TVertex>>? InspectVertex(TVertex vertex)
        {
            foreach (var child in graph.ChildrenOf(vertex))
            {
                if (verticesVisited.Contains(child))
                    continue;

                verticesVisited.Add(child);

                if (child.Equals(target))
                    return new[] { new DirectedEdge<TVertex>(vertex, child) };

                var childResult = InspectVertex(child);

                if (childResult is not null)
                    return childResult.Prepend(new DirectedEdge<TVertex>(vertex, child));
            }

            return null;
        }

        var result = InspectVertex(start);

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