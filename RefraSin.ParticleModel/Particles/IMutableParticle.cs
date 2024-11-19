using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public interface IMutableParticle<TNode> : IParticle<TNode>
    where TNode : IParticleNode
{
    IParticleSurface<TNode> Surface { get; }
}
