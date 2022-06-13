using System;
using System.Collections.Generic;
using System.Linq;
using IMF.Statistics.DistributedProperties;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.SinteringProcesses;

namespace RefraSin.Core.ParticleSources
{
    /// <summary>
    ///     Represents a mixture of distinct powders.
    /// </summary>
    public class PowderMixture : IPowderMixture
    {
        /// <summary>
        ///     Creates a new instance with the specified sequence of fractions.
        ///     A <see cref="CategoricalProperty{T}"/> is created automatically from the sequence.
        /// </summary>
        /// <param name="fractions">sequence of tuples with fraction amount and powder definition, the fractions are not required to sum up to 1</param>
        /// <param name="label">label for human identification</param>
        public PowderMixture(IEnumerable<(double fraction, IPowder powder)> fractions, string label = "")
        {
            Label = label;
            Fractions = new CategoricalProperty<IPowder>(
                fractions.Select(item => new Category<IPowder>(item.fraction, item.powder))
            );
        }

        /// <summary>
        ///     Creates a new instance with the specified fractions distributed property.
        /// </summary>
        /// <param name="fractions">should be most likely a <see cref="CategoricalProperty{T}"/> instance</param>
        /// <param name="label">label for human identification</param>
        public PowderMixture(IDistributedProperty<IPowder> fractions, string label = "")
        {
            Fractions = fractions;
            Label = label;
        }

        /// <inheritdoc />
        public IDistributedProperty<IPowder> Fractions { get; }

        /// <inheritdoc />
        public Particle GetParticle(ISinteringProcess process, int surfaceNodeCount) =>
            Fractions.Sample().GetParticle(process, surfaceNodeCount);

        /// <inheritdoc />
        public Particle GetParticle(ISinteringProcess process,
            SurfaceNodeCountFunction surfaceNodeCountFunction) =>
            Fractions.Sample().GetParticle(process, surfaceNodeCountFunction);

        /// <inheritdoc />
        public IEnumerable<Particle> GetParticles(ISinteringProcess process,
            SurfaceNodeCountFunction surfaceNodeCountFunction)
        {
            while (true)
            {
                yield return GetParticle(process, surfaceNodeCountFunction);
            }
        }

        /// <inheritdoc />
        public string Label { get; }
    }
}