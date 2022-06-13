using System;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.SinteringProcesses;

namespace RefraSin.Core.ParticleSources
{
    /// <summary>
    ///     Interface for classes, that deliver randomized <see cref="Particle" /> instances.
    /// </summary>
    public interface IRandomizedParticleSource : IParticleSource
    {
        /// <summary>
        /// Get an infinite sequence of randomized particles.
        /// </summary>
        /// <param name="process">reference to the sintering process instance</param>
        /// <param name="surfaceNodeCountFunction">function to determine the count of surface knots</param>
        public IEnumerable<Particle> GetParticles(ISinteringProcess process, SurfaceNodeCountFunction surfaceNodeCountFunction);
    }
}