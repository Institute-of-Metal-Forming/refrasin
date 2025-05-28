using RefraSin.Coordinates;
using RefraSin.Coordinates.Cartesian;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public record ParticleWithMaterial<TNode> : Particle<TNode>, IParticleWithMaterial
    where TNode : IParticleNode
{
    public ParticleWithMaterial(
        Guid id,
        ICartesianPoint centerCoordinates,
        Angle rotationAngle,
        IParticleMaterial material,
        Func<IParticle<TNode>, IEnumerable<TNode>> nodesFactory
    )
        : base(id, centerCoordinates, rotationAngle, material.Id, nodesFactory)
    {
        Material = material;
    }

    public static new ParticleWithMaterial<TNode> FromTemplate<TParticle>(
        TParticle template,
        Func<IParticleNode, IParticle<TNode>, TNode> nodeSelector
    )
        where TParticle : IParticle<IParticleNode>, IParticleWithMaterial
    {
        return new ParticleWithMaterial<TNode>(
            template.Id,
            template.Coordinates,
            template.RotationAngle,
            template.Material,
            particle => template.Nodes.Select(n => nodeSelector(n, particle))
        );
    }

    public IParticleMaterial Material { get; }
}
