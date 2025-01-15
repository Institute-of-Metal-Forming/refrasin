namespace RefraSin.Graphs;

public static class EdgeExtensions
{
    public static bool IsFrom<TVertex>(this IEdge<TVertex> self, TVertex from)
        where TVertex : IVertex => self.From.Equals(from);

    public static bool IsTo<TVertex>(this IEdge<TVertex> self, TVertex to)
        where TVertex : IVertex => self.To.Equals(to);

    public static bool IsAt<TVertex>(this IEdge<TVertex> self, TVertex vertex)
        where TVertex : IVertex => self.IsFrom(vertex) || self.IsTo(vertex);

    public static bool IsFromTo<TVertex>(this IEdge<TVertex> self, TVertex from, TVertex to)
        where TVertex : IVertex => self.IsFrom(from) && self.IsTo(to);

    public static bool IsBetween<TVertex>(this IEdge<TVertex> self, TVertex from, TVertex to)
        where TVertex : IVertex => self.IsFromTo(from, to) || self.IsFromTo(to, from);

    public static bool IsFrom(this IEdge self, Guid from) => self.From == from;

    public static bool IsTo(this IEdge self, Guid to) => self.To == to;

    public static bool IsAt(this IEdge self, Guid vertex) =>
        self.IsFrom(vertex) || self.IsTo(vertex);

    public static bool IsFromTo(this IEdge self, Guid from, Guid to) =>
        self.IsFrom(from) && self.IsTo(to);

    public static bool IsBetween(this IEdge self, Guid from, Guid to) =>
        self.IsFromTo(from, to) || self.IsFromTo(to, from);

    public static TVertex Other<TVertex>(this IEdge<TVertex> self, TVertex fromOrTo)
        where TVertex : IVertex
    {
        if (self.From.Equals(fromOrTo))
            return self.To;
        if (self.To.Equals(fromOrTo))
            return self.From;
        throw new ArgumentException($"Given fromOrTo {fromOrTo} is not part of edge {self}.");
    }

    public static TResult IfVertexIs<TVertex, TResult>(
        this IEdge<TVertex> self,
        TVertex vertex,
        Func<TResult> fromCase,
        Func<TResult> toCase
    )
        where TVertex : IVertex
    {
        if (self.From.Equals(vertex))
            return fromCase();
        if (self.To.Equals(vertex))
            return toCase();
        throw new ArgumentException($"Given fromOrTo {vertex} is not part of edge {self}.");
    }
}
