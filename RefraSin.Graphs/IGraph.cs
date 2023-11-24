using MoreLinq.Extensions;

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

    public IRootedGraph RootTo(IVertex vertex);

    public IRootedGraph RootOptimal() => Vertices.Select(RootTo).Minima(g => g.Depth).First();
}

public interface IRootedGraph : IGraph
{
    IVertex Root { get; }

    int Depth { get; }
}