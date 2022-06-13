using System;
using System.Collections.Generic;
using System.Linq;
using IMF.Statistics.DistributedProperties;
using MathNet.Numerics;
using MathNet.Numerics.Random;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.SinteringProcesses;

namespace RefraSin.Core.ParticleSources
{
    /// <summary>
    ///     Defines the particle property distributions of a powder.
    /// </summary>
    public class TrigonometricPowder : IPowder
    {
        /// <summary>
        /// Creates anew instance with the specified material data and the distributed shape parameters.
        /// Se <see cref="TrigonometricParticleSource.ParticleShapeRadius"/> for explanation of the shape parameters.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="baseRadius"></param>
        /// <param name="ovality"></param>
        /// <param name="peakHeight"></param>
        /// <param name="peakCount"></param>
        /// <param name="label">label for human identification</param>
        public TrigonometricPowder(Material material, IDistributedProperty<double> baseRadius, IDistributedProperty<double> ovality,
            IDistributedProperty<double> peakHeight, IDistributedProperty<uint> peakCount, string label = "")
        {
            BaseRadius = baseRadius;
            Ovality = ovality;
            PeakHeight = peakHeight;
            PeakCount = peakCount;
            Label = label;
            Material = material;
        }

        /// <summary>
        ///     Mean particle size distribution.
        /// </summary>
        public IDistributedProperty<double> BaseRadius { get; }

        /// <summary>
        ///     Particle ovality distribution.
        /// </summary>
        public IDistributedProperty<double> Ovality { get; }

        /// <summary>
        ///     Particle peak height distribution.
        /// </summary>
        public IDistributedProperty<double> PeakHeight { get; }

        /// <summary>
        ///     Particle peak count distribution.
        /// </summary>
        public IDistributedProperty<uint> PeakCount { get; }

        /// <summary>
        ///     Material properties.
        /// </summary>
        public Material Material { get; }

        /// <inheritdoc />
        public Particle GetParticle(ISinteringProcess process, int surfaceNodeCount) =>
            new TrigonometricParticleSource
            (
                Material,
                BaseRadius.Sample(),
                PeakHeight.Sample(),
                Ovality.Sample(),
                PeakCount.Sample()
            ).GetParticle(process, surfaceNodeCount);

        /// <inheritdoc />
        public Particle GetParticle(ISinteringProcess process,
            SurfaceNodeCountFunction surfaceNodeCountFunction)
        {
            var baseRadius = BaseRadius.Sample();
            return new TrigonometricParticleSource
            (
                Material,
                baseRadius,
                PeakHeight.Sample(),
                Ovality.Sample(),
                PeakCount.Sample()
            ).GetParticle(process, surfaceNodeCountFunction(baseRadius));
        }

        /// <inheritdoc />
        public IEnumerable<Particle> GetParticles(ISinteringProcess process, SurfaceNodeCountFunction surfaceNodeCountFunction)
        {
            while (true) yield return GetParticle(process, surfaceNodeCountFunction);
        }

        /// <inheritdoc />
        public string Label { get; }
    }
}