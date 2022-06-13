using System;
using System.Linq;
using IMF.Coordinates;
using IMF.Coordinates.Polar;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.SinteringProcesses;
using static System.Math;
using static System.String;
using static MathNet.Numerics.Constants;

namespace RefraSin.Core.ParticleSources
{
    /// <summary>
    ///     Definiert ein Partikel.
    /// </summary>
    public class TrigonometricParticleSource : IParticleSource
    {
        /// <summary>
        /// Creates a new instance with the specified material data and shape parameters.
        /// See <see cref="ParticleShapeRadius"/> for explanation of shape parameters.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="baseRadius"></param>
        /// <param name="peakHeight"></param>
        /// <param name="ovality"></param>
        /// <param name="peakCount"></param>
        public TrigonometricParticleSource(Material material, double baseRadius, double peakHeight = 0, double ovality = 0, uint peakCount = 0)
        {
            BaseRadius = baseRadius;
            PeakHeight = peakHeight;
            Ovality = ovality;
            PeakCount = peakCount;
            Material = material;
        }

        /// <summary>
        ///     Mittlerer Radius des Partikels.
        /// </summary>
        public double BaseRadius { get; }

        /// <summary>
        ///     Höhe der Oberflächenspitzen relativ zum mittleren Radius.
        /// </summary>
        public double PeakHeight { get; }

        /// <summary>
        ///     Ovalität relativ zum mittleren Radius.
        /// </summary>
        public double Ovality { get; }

        /// <summary>
        ///     Anzahl der Spitzen.
        /// </summary>
        public uint PeakCount { get; }

        /// <summary>
        /// Material data.
        /// </summary>
        public Material Material { get; }

        /// <inheritdoc />
        public Particle GetParticle(ISinteringProcess process, int surfaceNodeCount)
        {
            return new(parent => Enumerable.Range(0, surfaceNodeCount)
                .Select(i =>
                {
                    var phi = 2 * i * Pi / surfaceNodeCount;
                    return new SurfaceNode(parent, new PolarPoint(
                        phi,
                        ParticleShapeRadius(phi, BaseRadius, Ovality, PeakHeight, PeakCount)
                    ));
                }), Material, process);
        }

        /// <inheritdoc />
        public Particle GetParticle(ISinteringProcess process,
            SurfaceNodeCountFunction surfaceNodeCountFunction) => GetParticle(process, surfaceNodeCountFunction(BaseRadius));

        /// <summary>
        ///     Calculates local radius at an given angle with the specified shape parameters.
        /// </summary>
        /// <remarks>
        /// <c>r(phi) = r0 * (1 + hp * cos(np * phi) + o * cos(2 * phi))</c>
        /// </remarks>
        /// <param name="phi">angle coordinate</param>
        /// <param name="baseRadius">base radius r0</param>
        /// <param name="ovality">ovality o [0, 1]</param>
        /// <param name="peakHeight">peak height hp [0, 1]</param>
        /// <param name="peakCount">peak count np</param>
        /// <returns></returns>
        public static double ParticleShapeRadius(double phi, double baseRadius,
            double ovality = 0, double peakHeight = 0, uint peakCount = 0) =>
            baseRadius * (1 + ovality * Cos(2 * phi) + peakHeight * Cos(peakCount * phi));
    }
}