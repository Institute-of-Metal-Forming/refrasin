namespace RefraSin.Graphs;

public interface IGraph
{
    int VertexCount { get; }

    int EdgeCount { get; }

    IEnumerable<IVertex> Vertices { get; }

    IEnumerable<IEdge> Edges { get; }

    IVertex Root { get; }

    int Depth { get; }

    public IEnumerable<IVertex> ParentsOf(IVertex vertex) =>
        Edges.Where(e => e.IsEdgeTo(vertex)).Select(e => e.Start == vertex ? e.End : e.Start);

    public IEnumerable<IVertex> ChildrenOf(IVertex vertex) =>
        Edges.Where(e => e.IsEdgeFrom(vertex)).Select(e => e.Start == vertex ? e.End : e.Start);
}

public interface IDirectedGraph : IGraph { }