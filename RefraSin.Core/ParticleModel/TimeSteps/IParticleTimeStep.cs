using System;
using System.Collections.Generic;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;

namespace RefraSin.Core.ParticleModel.TimeSteps;

public interface IParticleTimeStep
{
    /// <summary>
    /// Unique ID of the particle this time step belongs to.
    /// </summary>
    public Guid ParticleId { get; }

    /// <summary>
    /// Displacement of the particle center in radial direction.
    /// </summary>
    public double RadialDisplacement { get; }

    /// <summary>
    /// Displacement of the particle center in angle direction.
    /// </summary>
    public Angle AngleDisplacement { get; }

    /// <summary>
    /// Rotational displacement of the particle around its center.
    /// </summary>
    public Angle RotationDisplacement { get; }

    /// <summary>
    /// Total vector of particle center displacement.
    /// </summary>
    public PolarVector DisplacementVector { get; }

    /// <summary>
    /// Total volume change.
    /// </summary>
    public double VolumeChange { get; }

    /// <summary>
    /// List of this particle's node time steps.
    /// </summary>
    public IReadOnlyDictionary<Guid, INodeTimeStep> NodeTimeSteps { get; }
}