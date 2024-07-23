namespace RefraSin.Graphs;

public record Edge(Guid From, Guid To, bool IsDirected) : IEdge
{
    public Edge(IEdge edge) : this(edge.From, edge.To, edge.IsDirected) { }

    public Edge Reversed() => new(To, From, IsDirected);
    IEdge IEdge.Reversed() => Reversed();
}

public record Edge<TVertex>(TVertex From, TVertex To, bool IsDirected) : IEdge<TVertex>
    where TVertex : IVertex
{
    public Edge(IEdge<TVertex> edge) : this(edge.From, edge.To, edge.IsDirected) { }
    
    public Edge<TVertex> Reversed() => new(To, From, IsDirected);
    IEdge IEdge.Reversed() => Reversed();
    IEdge<TVertex> IEdge<TVertex>.Reversed() => Reversed();
}