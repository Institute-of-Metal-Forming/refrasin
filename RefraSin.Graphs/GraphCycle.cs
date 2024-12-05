namespace RefraSin.Graphs;

public record GraphCycle<TVertex, TEdge>(
    TVertex Start,
    TVertex End,
    IEnumerable<TEdge> FirstPath,
    IEnumerable<TEdge> SecondPath
) : IGraphCycle<TVertex, TEdge>
    where TVertex : IVertex
    where TEdge : IEdge<TVertex> { }
