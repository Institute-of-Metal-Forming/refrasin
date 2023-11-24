namespace RefraSin.Graphs;

public interface IGraph
{
    ISet<IVertex> Vertices { get; }

    ISet<IEdge> Edges { get; }

    int VertexCount => Vertices.Count();

    int EdgeCount => Edges.Count();

    public IEnumerable<IVertex> ParentsOf(IVertex vertex) =>
        Edges.Where(e => e.IsEdgeTo(vertex)).Select(e => e.Start == vertex ? e.End : e.Start);

    public IEnumerable<IVertex> ChildrenOf(IVertex vertex) =>
        Edges.Where(e => e.IsEdgeFrom(vertex)).Select(e => e.Start == vertex ? e.End : e.Start);

    public IEdge Edge(IVertex from, IVertex to) => Edges.First(e => e.Start == from && e.End == to);

    public IEnumerable<(IEdge, IVertex)> DepthFirstSearch(IVertex start)
    {
        var verticesVisited = Vertices.ToDictionary(v => v, _ => false);
        verticesVisited[start] = true;

        var stack = new Stack<IVertex>();
        stack.Push(start);

        while (stack.TryPop(out var current))
        {
            foreach (var child in ChildrenOf(current).Reverse())
            {
                if (verticesVisited[child])
                    continue;

                verticesVisited[child] = true;

                stack.Push(child);
                yield return (Edge(current, child), child);
            }
        }
    }

    public IEnumerable<(IEdge, IVertex)> BreadthFirstSearch(IVertex start)
    {
        var verticesVisited = Vertices.ToDictionary(v => v, _ => false);
        verticesVisited[start] = true;

        var queue = new Queue<IVertex>();
        queue.Enqueue(start);

        while (queue.TryDequeue(out var current))
        {
            foreach (var child in ChildrenOf(current))
            {
                if (verticesVisited[child])
                    continue;

                verticesVisited[child] = true;

                queue.Enqueue(child);
                yield return (Edge(current, child), child);
            }
        }
    }

}

public interface IRootedGraph : IGraph
{
    IVertex Root { get; }

    int Depth { get; }

    public IEnumerable<(IEdge, IVertex)> DepthFirstSearch() => DepthFirstSearch(Root);

    public IEnumerable<(IEdge, IVertex)> BreadthFirstSearch() => BreadthFirstSearch(Root);
}