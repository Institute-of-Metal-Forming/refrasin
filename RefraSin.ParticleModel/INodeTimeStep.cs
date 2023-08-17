using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

public interface INodeTimeStep
{
    /// <summary>
    /// Unique ID of the node this time step belongs to.
    /// </summary>
    public Guid NodeId { get; }

    /// <summary>
    /// Distance od displacement in normal direction to the surface.
    /// </summary>
    public double NormalDisplacement { get; }

    /// <summary>
    /// Distance od displacement in tangential direction to the surface.
    /// </summary>
    public double TangentialDisplacement { get; }

    /// <summary>
    /// Total vector of displacement in terms of local particle coordinates.
    /// </summary>
    public PolarVector DisplacementVector { get; }

    /// <summary>
    /// Diffusional flow to/from the neighbor nodes.
    /// </summary>
    public ToUpperToLower DiffusionalFlow { get; }

    /// <summary>
    /// Diffusional flow to/from the environment (e.g. matrix, neighbor particle).
    /// </summary>
    public double OuterDiffusionalFlow { get; }

    /// <summary>
    /// Total volume change.
    /// </summary>
    public double VolumeChange { get; }
}