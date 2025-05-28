using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Cartesian;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.Particles;
using RefraSin.Vertex;

namespace RefraSin.Compaction.ParticleModel;

internal class ParticleAgglomerate : ICartesianCoordinateSystem, IAgglomerate
{
    public ParticleAgglomerate(Guid id, IEnumerable<IParticle<IParticleNode>> particles)
    {
        Id = id;
        var particlesArray = particles as IParticle<IParticleNode>[] ?? particles.ToArray();
        Coordinates = particlesArray
            .Select(p => p.Coordinates.Absolute)
            .Centroid<AbsolutePoint, AbsoluteVector>();
        Particles = particlesArray
            .Select(p => new AgglomeratedParticle(p, this))
            .ToReadOnlyVertexCollection();
        Surface = new ParticleSurface<Node>(CreateOuterSurface());
    }

    IEnumerable<Node> CreateOuterSurface()
    {
        var allNodes = Particles.SelectMany(p => p.Nodes).ToArray();
        var firstNode =
            allNodes.MaxBy(n => n.Coordinates.DistanceTo(Coordinates))
            ?? throw new InvalidOperationException("outermost node could not be determined");
        var currentNode = firstNode;

        while (true)
        {
            yield return new Node(
                currentNode.Id,
                currentNode.Coordinates.Absolute,
                currentNode.Type,
                this
            );

            if (currentNode is { Type: NodeType.Neck, Upper.Type: NodeType.GrainBoundary })
            {
                currentNode = currentNode.FindContactedNodeByCoordinates(allNodes);
                continue;
            }

            currentNode = currentNode.Upper;

            if (currentNode == firstNode)
                break;
        }
    }

    /// <inheritdoc />
    public AbsolutePoint Coordinates { get; private set; }

    /// <inheritdoc />
    public Guid MaterialId => Guid.Empty;

    /// <inheritdoc cref="IMoveableParticle.RotationAngle" />
    public Angle RotationAngle { get; private set; }

    ICartesianPoint IParticle.Coordinates => Coordinates;

    /// <inheritdoc />
    IPoint ICoordinateSystem.Origin => Coordinates;

    public IReadOnlyVertexCollection<AgglomeratedParticle> Particles { get; }

    IEnumerable<IMutableParticle<Node>> IAgglomerate.Elements => Particles;

    /// <inheritdoc />
    public void MoveTowards(IMoveableParticle target, double distance) =>
        MoveTowards(target.Coordinates, distance);

    public void MoveTowards(IPoint target, double distance)
    {
        var direction = (target.Absolute - Coordinates).Direction;
        var movement = distance * direction;

        Coordinates += movement;
    }

    /// <inheritdoc />
    public void Rotate(Angle angle)
    {
        RotationAngle += angle;
    }

    /// <inheritdoc />
    public double XScale => 1;

    /// <inheritdoc />
    public double YScale => 1;

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public IReadOnlyParticleSurface<Node> Nodes => Surface;

    /// <inheritdoc />
    public IParticleSurface<Node> Surface { get; }
}
