using System.Linq;
using IMF.Enumerables;
using IMF.Utils;
using MoreLinq.Extensions;
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
    public class RandomizedNecklaceTreeSource : IParticleTreeSource
    {
        /// <summary>
        /// Creates a new instance with the specified particle source and a count of ring particles.
        /// </summary>
        /// <param name="randomizedParticleSource"></param>
        /// <param name="ringParticleCount"></param>
        public RandomizedNecklaceTreeSource(IRandomizedParticleSource randomizedParticleSource, uint ringParticleCount = 5)
        {
            RandomizedParticleSource = randomizedParticleSource;
            RingParticleCount = ringParticleCount;
        }

        /// <summary>
        /// Randomized source for particles.
        /// </summary>
        public IRandomizedParticleSource RandomizedParticleSource { get; set; }

        /// <summary>
        /// Gets or sets the count of particles in the ring. Note that the root particle is <em>not</em> included in this count.
        /// </summary>
        public uint RingParticleCount { get; }

        /// <inheritdoc />
        public Tree<Particle> GetParticleTree(ISinteringProcess process,
            SurfaceNodeCountFunction surfaceNodeCountFunction)
        {
            var samples = RandomizedParticleSource.GetParticles(process, surfaceNodeCountFunction).Take((int)(RingParticleCount + 1))
                .Select(p => (particle: p, maxRadius: p.Surface.Max(k => k.Coordinates.R))).ToArray();

            var root = samples.MaxBy(x => x.maxRadius).First();
            var necklaceMembers = samples.Where(x => x.particle != root.particle).ToArray();

            var totalMeanRadius = necklaceMembers.Sum(x => x.maxRadius);

            var phi = -root.particle.RotationAngle;
            foreach (var (particle, maxRadius) in necklaceMembers)
            {
                var phiStep = Pi * maxRadius / totalMeanRadius;
                phi += phiStep;
                particle.CenterCoordinates.Phi = phi;
                particle.CenterCoordinates.R = root.maxRadius + maxRadius;
                root.particle.Children.Add(particle);
                phi += phiStep;
            }

            return new(root.particle);
        }
    }
}