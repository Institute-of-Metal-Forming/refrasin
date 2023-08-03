using System.Collections.Generic;
using IMF.Coordinates;
using IMF.Coordinates.Polar;

namespace RefraSin.Core.Solver.TimeSteps;

public interface IParticleTimeStep
{
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
    public IList<INodeTimeStep> NodeTimeSteps { get; }
}