using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public record ParticleMeasures(
    double MinRadius,
    double MaxRadius,
    double MinX,
    double MaxX,
    double MinY,
    double MaxY
) : IParticleMeasures
{
    public ParticleMeasures(IParticle<IParticleNode> particle)
        : this(
            particle.Nodes.Min(n => n.Coordinates.R),
            particle.Nodes.Max(n => n.Coordinates.R),
            particle.Nodes.Min(n => n.Coordinates.Absolute.X),
            particle.Nodes.Max(n => n.Coordinates.Absolute.X),
            particle.Nodes.Min(n => n.Coordinates.Absolute.Y),
            particle.Nodes.Max(n => n.Coordinates.Absolute.Y)
        ) { }
}
