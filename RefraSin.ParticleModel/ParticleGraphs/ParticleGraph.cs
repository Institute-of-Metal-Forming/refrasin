namespace RefraSin.ParticleModel.ParticleGraphs;

public class ParticleGraph
{
    private readonly List<IVertex> _vertices;
    private readonly List<IEdge> _edges;

    public ParticleGraph(IEnumerable<IVertex> vertices, IEnumerable<IEdge> edges) : this(vertices.ToArray(), edges.ToArray()) { }

    public ParticleGraph(IReadOnlyList<IVertex> vertices, IReadOnlyList<IEdge> edges)
    {
        if (vertices.Count == 0)
            throw new ArgumentException("Vertices must not be empty.");

        _vertices = new List<IVertex>(vertices.Count);
        _edges = new List<IEdge>(edges.Count);

        if (edges.Count != 0)
        {
            var groupedEdges = edges.GroupBy(e => e.Start).ToDictionary(g => g.Key, g => g.OrderBy(e => e.Angle).ToArray());

            Root = vertices.First(v => groupedEdges.ContainsKey(v.Id));

            var verticesToVisit = vertices.ToDictionary(v => v.Id);

            var queue = new Queue<IVertex>();
            queue.Enqueue(Root);

            while (queue.TryDequeue(out var vertex))
            {
                foreach (var edge in groupedEdges[vertex.Id])
                {
                    _edges.Add(edge);

                    if (verticesToVisit.Remove(edge.End, out var end))
                    {
                        queue.Enqueue(end);
                        _vertices.Add(end);
                    }
                }
            }

            if (verticesToVisit.Count != 0)
                throw new ArgumentException("The given edges do not connect all given vertices.");
        }
        else if (vertices.Count == 1)
        {
            Root = vertices[0];
        }
        else
        {
            throw new ArgumentException("Edges must not be empty if count of vertices is larger than 1.");
        }
    }

    public IVertex Root { get; }

    public IEnumerable<IVertex> Vertices => _vertices;

    public IEnumerable<IEdge> Edges => _edges;
}