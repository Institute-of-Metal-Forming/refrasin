using RefraSin.MaterialData;

namespace RefraSin.ParticleModel.Particles;

public interface IParticleWithMaterial : IParticle
{
    IParticleMaterial Material { get; }
}
