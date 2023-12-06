using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Graphs;

namespace RefraSin.ParticleModel;

public class Particle : IParticle
{
    private readonly NodeCollection<INode> _nodes;

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

    public Particle(Guid id, AbsolutePoint centerCoordinates, Angle rotationAngle, Guid materialId, IReadOnlyList<INode> nodes)
    {
        Id = id;
        CenterCoordinates = centerCoordinates;
        RotationAngle = rotationAngle;
        MaterialId = materialId;
        _nodes = new(nodes);
    }

    public IReadOnlyNodeCollection<INode> Nodes => _nodes;
    public Guid Id { get; }
    public AbsolutePoint CenterCoordinates { get; }
    public Angle RotationAngle { get; }
    public Guid MaterialId { get; }

    /// <inheritdoc />
    public bool Equals(IVertex other) => other is IParticle && Id == other.Id;
}