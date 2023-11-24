namespace RefraSin.Graphs;

public interface IGraphSearch<TVertex> where TVertex : IVertex
{
    TVertex Start { get; }

    IEnumerable<DirectedEdge<TVertex>> ExploredEdges { get; }
}