using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.System;

namespace RefraSin.ParticleModel.Remeshing;

public interface IParticleRemesher : IParticleSystemRemesher
{
    IParticle<IParticleNode> Remesh(IParticle<IParticleNode> particle);

    IParticleSystem<IParticle<IParticleNode>, IParticleNode> IParticleSystemRemesher.RemeshSystem(IParticleSystem<IParticle<IParticleNode>, IParticleNode> system) =>
        new ParticleSystem<IParticle<IParticleNode>, IParticleNode>(system.Particles.Select(Remesh));
}