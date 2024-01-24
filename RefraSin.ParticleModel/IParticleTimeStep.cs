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
    /// Displacement of the particle center in horizontal (x) direction.
    /// </summary>
    public double HorizontalDisplacement { get; }

    /// <summary>
    /// Displacement of the particle center in vertical (y) direction.
    /// </summary>
    public double VerticalDisplacement { get; }

    /// <summary>
    /// Rotational displacement of the particle around its center.
    /// </summary>
    public Angle RotationDisplacement { get; }

    /// <summary>
    /// List of this particle's node time steps.
    /// </summary>
    public IReadOnlyDictionary<Guid, INodeTimeStep> NodeTimeSteps { get; }
}