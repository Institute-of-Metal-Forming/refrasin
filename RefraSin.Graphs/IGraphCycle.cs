namespace RefraSin.Graphs;

public interface IGraphCycle<TVertex, TEdge>
    where TVertex : IVertex
    where TEdge : IEdge<TVertex>
{
    TVertex Start { get; }
    TVertex End { get; }

    IEnumerable<TEdge> FirstPath { get; }
    IEnumerable<TEdge> SecondPath { get; }
}
