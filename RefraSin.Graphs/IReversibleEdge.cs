namespace RefraSin.Graphs;

public interface IReversibleEdge<out TEdge>
    where TEdge : IEdge
{
    TEdge Reversed();
}
