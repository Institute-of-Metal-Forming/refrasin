namespace RefraSin.Graphs;

public static class EdgeExtensions
{
    public static bool IsEdgeFrom(this IEdge edge, IVertex from)
    {
        if (edge.IsDirected)
        {
            return edge.Start == from;
        }

        return edge.Start == from || edge.End == from;
    }

    public static bool IsEdgeTo(this IEdge edge, IVertex to)
    {
        if (edge.IsDirected)
        {
            return edge.End == to;
        }

        return edge.Start == to || edge.End == to;
    }

    public static bool IsEdgeFromTo(this IEdge edge, IVertex from, IVertex to) => edge.IsEdgeFrom(from) && edge.IsEdgeTo(to);
}