using RefraSin.ParticleModel.Particles;

namespace RefraSin.Compaction.ParticleModel;

internal interface IAgglomerate : IMoveableParticle
{
    IEnumerable<IMutableParticle<Node>> Elements { get; }
}
