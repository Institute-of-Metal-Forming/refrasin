using System;
using System.Globalization;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;
using RefraSin.Core.ParticleModel.Interfaces;

namespace RefraSin.Core.ParticleModel.States
{
    /// <summary>
    /// Represents a read-only state snapshot of a node.
    /// </summary>
    public abstract class NodeState : INode
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public Guid ParticleId { get; }

        /// <inheritdoc />
        public PolarPoint Coordinates { get; }

        /// <inheritdoc />
        public AbsolutePoint AbsoluteCoordinates { get; }

        /// <inheritdoc />
        public ToUpperToLower SurfaceDistance { get; }

        /// <inheritdoc />
        public ToUpperToLowerAngle SurfaceRadiusAngle { get; }

        /// <inheritdoc />
        public ToUpperToLowerAngle AngleDistance { get; }

        /// <inheritdoc />
        public ToUpperToLower SurfaceDiffusionCoefficient { get; }

        /// <inheritdoc />
        public NormalTangential GibbsEnergyGradient { get; }

        /// <inheritdoc />
        public NormalTangential VolumeGradient { get; }

        /// <inheritdoc />
        public override string ToString() => $"{GetType()} {Id.ToShortString()} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)}";
    }
}