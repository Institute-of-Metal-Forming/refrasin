using IMF.Enumerables;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.ParticleSources;
using RefraSin.Core.SinteringProcesses;

namespace RefraSin.Core.ParticleTreeSources
{
    /// <summary>
    /// Interface for classes able to generate a tree of <see cref="Particle"/> instances.
    /// </summary>
    public interface IParticleTreeSource
    {
        /// <summary>
        /// Generates a tree of particles.
        /// </summary>
        /// <returns></returns>
        public Tree<Particle> GetParticleTree(ISinteringProcess process, SurfaceNodeCountFunction surfaceNodeCountFunction);
    }
}