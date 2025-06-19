using System.Globalization;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Cartesian;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.Compaction.ParticleModel;

internal class Particle : IMutableParticle<Node>, IMoveableParticle
{
    public Particle(IParticle<IParticleNode> template)
    {
        Id = template.Id;
        RotationAngle = template.RotationAngle;
        Coordinates = template.Coordinates.Absolute;
        MaterialId = template.MaterialId;
        Surface = new ParticleSurface<Node>(template.Nodes.Select(n => new Node(n, this)));
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public Angle RotationAngle { get; private set; }

    public AbsolutePoint Coordinates { get; private set; }

    ICartesianPoint IParticle.Coordinates => Coordinates;

    /// <inheritdoc />
    public Guid MaterialId { get; }

    /// <inheritdoc />
    public IReadOnlyParticleSurface<Node> Nodes => Surface;

    public void MoveTowards(IMoveableParticle target, double distance) =>
        MoveTowards(target.Coordinates, distance);

    public void MoveTowards(IPoint target, double distance)
    {
        var direction = (target.Absolute - Coordinates).Direction;
        var movement = distance * direction;

        Coordinates += movement;
    }

    public void MoveBy(IVector direction, double distance)
    {
        var movement = distance * direction.Direction.Absolute;
        Coordinates += movement;
    }

    /// <inheritdoc />
    public void Rotate(Angle angle)
    {
        RotationAngle += angle;
    }

    public override string ToString() =>
        $"{nameof(Particle)} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)}";

    /// <inheritdoc />
    public IParticleSurface<Node> Surface { get; }
}
