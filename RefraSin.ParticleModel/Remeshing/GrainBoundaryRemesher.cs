using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.System;

namespace RefraSin.ParticleModel.Remeshing;

public class GrainBoundaryRemesher : IParticleSystemRemesher
{
    /// <inheritdoc />
    public IParticleSystem<IParticle<IParticleNode>, IParticleNode> RemeshSystem(IParticleSystem<IParticle<IParticleNode>, IParticleNode> system) => throw new NotImplementedException();
}