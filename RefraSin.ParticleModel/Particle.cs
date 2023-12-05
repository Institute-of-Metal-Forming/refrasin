using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Graphs;

namespace RefraSin.ParticleModel;

public record Particle(Guid Id, AbsolutePoint CenterCoordinates, Angle RotationAngle, Guid MaterialId, IReadOnlyList<INode> Nodes)
    : IParticle
{
    public Particle(IParticle template) : this(
        template.Id,
        template.CenterCoordinates,
        template.RotationAngle,
        template.MaterialId,
        template.Nodes.Select(node => node switch
        {
            INeckNode neckNode                   => new NeckNode(neckNode),
            IGrainBoundaryNode grainBoundaryNode => new GrainBoundaryNode(grainBoundaryNode),
            ISurfaceNode surfaceNode             => new SurfaceNode(surfaceNode),
            _                                    => new Node(node)
        }).ToArray()
    ) { }

    /// <inheritdoc />
    public INode this[int i] => i >= 0 ? Nodes[(i % Nodes.Count)] : Nodes[^-(i % Nodes.Count)];

    /// <inheritdoc />
    public INode this[Guid nodeId] => Nodes.FirstOrDefault(n => n.Id == nodeId) ??
                                      throw new IndexOutOfRangeException($"A node with ID {nodeId} is not present in this particle.");

    /// <inheritdoc />
    public bool Equals(IVertex other) => other is IParticle && Id == other.Id;
}