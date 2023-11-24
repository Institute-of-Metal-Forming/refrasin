namespace RefraSin.Graphs;

public static class GraphExtensions
{
    public static IEnumerable<IVertex> ParentsOf(this IGraph graph, IVertex vertex) =>
        graph.Edges.Where(e => e.IsEdgeTo(vertex)).Select(e => e.Start == vertex ? e.End : e.Start);

    public static IEnumerable<IVertex> ChildrenOf(this IGraph graph, IVertex vertex) =>
        graph.Edges.Where(e => e.IsEdgeFrom(vertex)).Select(e => e.Start == vertex ? e.End : e.Start);

    public static IEnumerable<IVertex> DepthFirstSearch(this IGraph graph, IVertex start)
    {
        var verticesVisited = graph.Vertices.ToDictionary(v => v, _ => false);

        return DepthFirstSearch(graph, start, verticesVisited);
    }

    private static IEnumerable<IVertex> DepthFirstSearch(IGraph graph, IVertex current, Dictionary<IVertex, bool> verticesVisited)
    {
        if (verticesVisited[current])
            yield break;

        verticesVisited[current] = true;

        yield return current;

        foreach (var child in graph.ChildrenOf(current))
        {
            foreach (var vertex in DepthFirstSearch(graph, child, verticesVisited))
            {
                yield return vertex;
            }
        }
    }

    public static IEnumerable<IVertex> BreadthFirstSearch(this IGraph graph, IVertex start)
    {
        var verticesVisited = graph.Vertices.ToDictionary(v => v, _ => false);
        verticesVisited[start] = true;

        var queue = new Queue<IVertex>();
        queue.Enqueue(start);

        while (queue.TryDequeue(out var vertex))
        {
            foreach (var child in graph.ChildrenOf(vertex))
            {
                if (verticesVisited[child])
                    continue;

                verticesVisited[child] = true;

                queue.Enqueue(child);
                yield return child;
            }
        }
    }
}