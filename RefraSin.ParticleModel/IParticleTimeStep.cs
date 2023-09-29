using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

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
    /// List of this particle's node time steps.
    /// </summary>
    public IReadOnlyDictionary<Guid, INodeTimeStep> NodeTimeSteps { get; }
}