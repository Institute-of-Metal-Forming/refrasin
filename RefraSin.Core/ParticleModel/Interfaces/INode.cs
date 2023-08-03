using System;
using IMF.Coordinates.Absolute;
using IMF.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;

namespace RefraSin.Core.ParticleModel.Interfaces
{
    /// <summary>
    /// Interface representing a node of a particle surface.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// ID of the node.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// ID of the parent particle.
        /// </summary>
        public Guid ParticleId { get; }

        /// <summary>
        /// Polar coordinates in the particle's coordinate system (<see cref="Particle.LocalCoordinateSystem"/>).
        /// </summary>
        public PolarPoint Coordinates { get; }

        /// <summary>
        /// Absolute coordinates.
        /// </summary>
        public AbsolutePoint AbsoluteCoordinates { get; }

        /// <summary>
        /// Length of the surface lines to neighbor nodes.
        /// </summary>
        public ToUpperToLower SurfaceDistance { get; }

        /// <summary>
        /// Angle between radial vector and surface line.
        /// </summary>
        public ToUpperToLowerAngle SurfaceRadiusAngle { get; }

        /// <summary>
        /// Angle distance to neighbor nodes.
        /// </summary>
        public ToUpperToLowerAngle AngleDistance { get; }

        /// <summary>
        /// Diffusion coefficient at the surface lines to the neighbor nodes.
        /// </summary>
        public ToUpperToLower SurfaceDiffusionCoefficient { get; }

        /// <summary>
        /// Gradient of Gibbs energy for node shifting in normal and tangential direction.
        /// </summary>
        public NormalTangential GibbsEnergyGradient { get; }

        /// <summary>
        /// Gradient of volume for node shifting in normal and tangential direction.
        /// </summary>
        public NormalTangential VolumeGradient { get; }
    }
}