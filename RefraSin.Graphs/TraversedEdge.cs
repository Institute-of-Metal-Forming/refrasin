namespace RefraSin.Graphs;

public record TraversedEdge<TVertex>(
    TVertex Start,
    TVertex End,
    bool EndVertexAlreadyVisited
) : DirectedEdge<TVertex>(Start, End) where TVertex : IVertex { }