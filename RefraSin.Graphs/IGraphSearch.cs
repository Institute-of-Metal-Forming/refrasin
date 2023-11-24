namespace RefraSin.Graphs;

public interface IGraphSearch
{
    IVertex Start { get; }

    IEnumerable<IEdge> ExploredEdges { get; }
}