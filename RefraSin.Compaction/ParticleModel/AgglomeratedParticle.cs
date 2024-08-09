using RefraSin.Coordinates;
using RefraSin.Coordinates.Cartesian;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.Compaction.ParticleModel;

internal class AgglomeratedParticle : IMutableParticle<Node>
{
    public AgglomeratedParticle(IParticle<IParticleNode> template, ParticleAgglomerate agglomerate)
    {
        Id = template.Id;
        RotationAngle = template.RotationAngle;
        Coordinates = new CartesianPoint(template.Coordinates, agglomerate);
        MaterialId = template.MaterialId;
        Surface = new ParticleSurface<Node>(template.Nodes.Select(n => new Node(n, this)));
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public Angle RotationAngle { get; }

    /// <inheritdoc />
    public ICartesianPoint Coordinates { get; }

    /// <inheritdoc />
    public Guid MaterialId { get; }

    /// <inheritdoc />
    public IReadOnlyParticleSurface<Node> Nodes => Surface;

    /// <inheritdoc />
    public IParticleSurface<Node> Surface { get; }
}
