namespace RefraSin.Graphs;

public class CycleFinder<TVertex, TEdge>
    where TEdge : IEdge<TVertex>
    where TVertex : IVertex
{
    private readonly IGraphCycle<TVertex, TEdge>[] _foundCycles;

    private CycleFinder(IGraphCycle<TVertex, TEdge>[] foundCycles)
    {
        _foundCycles = foundCycles;
    }

    public static CycleFinder<TVertex, TEdge> FindCycles(
        IGraph<TVertex, TEdge> graph,
        TVertex start
    ) => new(DoFindCycles(graph, start).ToArray());

    public static CycleFinder<TVertex, TEdge> FindCycles(IRootedGraph<TVertex, TEdge> graph) =>
        new(DoFindCycles(graph, graph.Root).ToArray());

    private static IEnumerable<IGraphCycle<TVertex, TEdge>> DoFindCycles(
        IGraph<TVertex, TEdge> graph,
        TVertex start
    )
    {
        var verticesVisited = new HashSet<TVertex>(graph.VertexCount) { start };
        var edgesTakenTo = new Dictionary<TVertex, TEdge>(graph.EdgeCount);

        var queue = new Queue<TVertex>();
        queue.Enqueue(start);

        while (queue.TryDequeue(out var current))
        {
            foreach (var edge in graph.EdgesFrom(current))
            {
                var child = edge.To;

                if (verticesVisited.Contains(child))
                {
                    yield return CreateGraphCycle(edge, edgesTakenTo);
                    continue;
                }

                verticesVisited.Add(child);
                edgesTakenTo.Add(child, edge);
                queue.Enqueue(child);
            }
        }
    }

    private static IGraphCycle<TVertex, TEdge> CreateGraphCycle(
        TEdge closingEdge,
        IReadOnlyDictionary<TVertex, TEdge> edgesTakeTo
    )
    {
        var firstPath = TraceBack(closingEdge.To, edgesTakeTo).Reverse().ToArray();
        var secondPath = TraceBack(closingEdge.From, edgesTakeTo)
            .Prepend(closingEdge)
            .Reverse()
            .ToArray();
        var firstCommon = firstPath.First(e1 => secondPath.Any(e2 => e1.From.Equals(e2.From))).From;

        return new GraphCycle<TVertex, TEdge>(firstCommon, closingEdge.To, firstPath, secondPath);
    }

    private static IEnumerable<TEdge> TraceBack(
        TVertex start,
        IReadOnlyDictionary<TVertex, TEdge> edgesTakenTo
    )
    {
        var currentVertex = start;

        while (edgesTakenTo.TryGetValue(currentVertex, out var edge))
        {
            yield return edge;
            currentVertex = edge.From;
        }
    }

    public IEnumerable<IGraphCycle<TVertex, TEdge>> FoundCycles => _foundCycles;
}
