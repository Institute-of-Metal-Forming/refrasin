using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.SinteringProcesses;

namespace RefraSin.Core.ParticleSources
{
    /// <summary>
    ///     Interface for classes that are able to construct a <see cref="Particle" /> object.
    /// </summary>
    public interface IParticleSource
    {
        ///  <summary>
        ///      Creates an instance of <see cref="Particle" />.
        ///  </summary>
        ///  <param name="process">reference to the sintering process instance</param>
        ///  <param name="surfaceNodeCount">count of surface knots to generate</param>
        public Particle GetParticle(ISinteringProcess process, int surfaceNodeCount);

        /// <summary>
        /// Get a randomized particle.
        /// </summary>
        /// <param name="process">reference to the sintering process instance</param>
        /// <param name="surfaceNodeCountFunction">function to determine the count of surface knots</param>
        public Particle GetParticle(ISinteringProcess process, SurfaceNodeCountFunction surfaceNodeCountFunction);
    }
}