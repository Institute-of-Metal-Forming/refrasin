namespace RefraSin.Graphs;

public class TraversedEdge<TVertex> : DirectedEdge<TVertex> where TVertex : IVertex
{
    public TraversedEdge(TVertex start,
        TVertex end,
        bool endVertexAlreadyVisited) : base(start, end)
    {
        EndVertexAlreadyVisited = endVertexAlreadyVisited;
    }

    public void Deconstruct(out TVertex start, out TVertex end, out bool endVertexAlreadyVisited)
    {
        start = From;
        end = To;
        endVertexAlreadyVisited = EndVertexAlreadyVisited;
    }

    public bool EndVertexAlreadyVisited { get; }

    /// <inheritdoc />
    public override string ToString() =>
        $"TraversedEdge from {From} to {To}, {(EndVertexAlreadyVisited ? "end visited before" : "end not visited before")}";
}