namespace RefraSin.Graphs;

public interface IGraph
{
    int VertexCount { get; }

    int EdgeCount { get; }

    IEnumerable<IVertex> Vertices { get; }

    IEnumerable<IEdge> Edges { get; }

    IVertex Root { get; }

    int Depth { get; }
}

public interface IDirectedGraph : IGraph
{

}