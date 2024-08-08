using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Cartesian;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.Compaction.ParticleModel;

internal class Particle : IParticle<Node>, IRigidBody
{
    public Particle(IParticle<IParticleNode> template)
    {
        Id = template.Id;
        RotationAngle = template.RotationAngle;
        Coordinates = template.Coordinates.Absolute;
        MaterialId = template.MaterialId;
        Nodes = template.Nodes.Select(n => new Node(n, this)).ToReadOnlyParticleSurface();
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
    public IReadOnlyParticleSurface<Node> Nodes { get; }

    public void MoveTowards(IRigidBody target, double distance) =>
        MoveTowards(target.Coordinates, distance);

    public void MoveTowards(IPoint target, double distance)
    {
        var direction = target.Absolute - Coordinates;
        var movement = distance / direction.Norm * direction;

        Coordinates += movement;
    }

    /// <inheritdoc />
    public void Rotate(Angle angle)
    {
        RotationAngle += angle;
    }
}
