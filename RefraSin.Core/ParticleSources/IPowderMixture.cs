using IMF.Statistics.DistributedProperties;

namespace RefraSin.Core.ParticleSources
{
    /// <summary>
    ///     Interface for definitions of powders miced from distinct fractions.
    /// </summary>
    public interface IPowderMixture : IRandomizedParticleSource, ILabeled
    {
        /// <summary>
        ///     Fractions from which the powder is mixed.
        /// </summary>
        public IDistributedProperty<IPowder> Fractions { get; }
    }
}