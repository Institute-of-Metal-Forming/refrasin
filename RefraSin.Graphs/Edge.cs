namespace RefraSin.Graphs;

public record Edge(Guid From, Guid To) : IEdge, IReversibleEdge<Edge>
{
    public Edge(IEdge edge)
        : this(edge.From, edge.To) { }

    public Edge Reversed() => new(To, From);
}

public record Edge<TVertex>(TVertex From, TVertex To)
    : IEdge<TVertex>,
        IReversibleEdge<Edge<TVertex>>
    where TVertex : IVertex
{
    public Edge(IEdge<TVertex> edge)
        : this(edge.From, edge.To) { }

    public Edge<TVertex> Reversed() => new(To, From);
}
