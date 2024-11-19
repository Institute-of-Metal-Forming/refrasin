namespace RefraSin.Graphs;

public record Edge(Guid From, Guid To) : IEdge
{
    public Edge(IEdge edge)
        : this(edge.From, edge.To) { }

    public Edge Reversed() => new(To, From);

    IEdge IEdge.Reversed() => Reversed();
}

public record Edge<TVertex>(TVertex From, TVertex To) : IEdge<TVertex>
    where TVertex : IVertex
{
    public Edge(IEdge<TVertex> edge)
        : this(edge.From, edge.To) { }

    public Edge<TVertex> Reversed() => new(To, From);

    IEdge IEdge.Reversed() => Reversed();

    IEdge<TVertex> IEdge<TVertex>.Reversed() => Reversed();
}
