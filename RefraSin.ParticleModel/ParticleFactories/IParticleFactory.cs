using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParticleModel.ParticleFactories;

public interface IParticleFactory
{
    IParticle GetParticle();
}