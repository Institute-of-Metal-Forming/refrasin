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
}
