using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.ParticleFactories;

public interface IParticleFactory<out TParticle, out TNode>
    where TParticle : IParticle<TNode>
    where TNode : IParticleNode
{
    TParticle GetParticle(Guid? id = null);
}
