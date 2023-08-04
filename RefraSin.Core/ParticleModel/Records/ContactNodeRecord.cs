using System;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;
using RefraSin.Core.ParticleModel.Interfaces;

namespace RefraSin.Core.ParticleModel.Records
{
    /// <summary>
    /// Represents an immutable state of a contact node.
    /// </summary>
    public abstract record ContactNodeRecord : NodeRecord, IContactNode
    {
        /// <inheritdoc />
        protected ContactNodeRecord(IContactNode template) : base(template)
        {
            ContactedParticleId = template.ContactedParticleId;
            TransferCoefficient = template.TransferCoefficient;
        }

        /// <inheritdoc />
        protected ContactNodeRecord(Guid Id,
            Guid ParticleId,
            PolarPoint Coordinates,
            AbsolutePoint AbsoluteCoordinates,
            ToUpperToLower SurfaceDistance,
            ToUpperToLowerAngle SurfaceRadiusAngle,
            ToUpperToLowerAngle AngleDistance,
            ToUpperToLower Volume,
            NormalTangentialAngle SurfaceAngle,
            ToUpperToLower SurfaceEnergy,
            ToUpperToLower SurfaceDiffusionCoefficient,
            NormalTangential GibbsEnergyGradient,
            NormalTangential VolumeGradient,
            Guid contactedParticleId,
            double transferCoefficient
        ) : base(
            Id,
            ParticleId,
            Coordinates,
            AbsoluteCoordinates,
            SurfaceDistance,
            SurfaceRadiusAngle,
            AngleDistance,
            Volume,
            SurfaceAngle,
            SurfaceEnergy,
            SurfaceDiffusionCoefficient,
            GibbsEnergyGradient,
            VolumeGradient
        )
        {
            ContactedParticleId = contactedParticleId;
            TransferCoefficient = transferCoefficient;
        }

        /// <inheritdoc />
        public Guid ContactedParticleId { get; }

        /// <inheritdoc />
        public double TransferCoefficient { get; }
    }
}