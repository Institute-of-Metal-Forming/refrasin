using System.Linq;
using IMF.Enumerables;
using IMF.Utils;
using MathNet.Numerics.Random;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.ParticleSources;
using RefraSin.Core.SinteringProcesses;
using static MathNet.Numerics.Constants;

namespace RefraSin.Core.ParticleTreeSources
{
    /// <summary>
    /// Generates a particle tree by placing a necklace of particles around a center particle.
    /// Draws from a randomized particle source.
    /// The biggest particle is placed in the center.
    /// </summary>
    public class RandomizedPairTreeSource : IParticleTreeSource
    {
        /// <summary>
        /// Creates a new instance with the specified particle source.
        /// </summary>
        /// <param name="randomizedParticleSource"></param>
        public RandomizedPairTreeSource(IRandomizedParticleSource randomizedParticleSource)
        {
            RandomizedParticleSource = randomizedParticleSource;
        }

        /// <summary>
        /// Randomized source for particles.
        /// </summary>
        public IRandomizedParticleSource RandomizedParticleSource { get; }

        /// <inheritdoc />
        public Tree<Particle> GetParticleTree(ISinteringProcess process,
            SurfaceNodeCountFunction surfaceNodeCountFunction)
        {
            var particle1 = RandomizedParticleSource.GetParticle(process, surfaceNodeCountFunction);
            var particle2 = RandomizedParticleSource.GetParticle(process, surfaceNodeCountFunction);

            var particle1MaxRadius = particle1.SurfaceNodes.Max(k => k.Coordinates.R);
            var particle2MaxRadius = particle2.SurfaceNodes.Max(k => k.Coordinates.R);

            var rotationAngle1 = MersenneTwister.Default.NextDouble() * Pi2;
            var rotationAngle2 = MersenneTwister.Default.NextDouble() * Pi2;

            particle1.RotationAngle = rotationAngle1;
            particle2.RotationAngle = rotationAngle2;
            
            particle2.CenterCoordinates.Phi = -rotationAngle1;
            particle2.CenterCoordinates.R = 1.1 * (particle1MaxRadius + particle2MaxRadius);
            particle1.Children.Add(particle2);

            return new(particle1);
        }
    }
}