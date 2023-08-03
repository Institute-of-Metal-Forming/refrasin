using IMF.Coordinates.Polar;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.ParticleModel.HelperTypes;

namespace RefraSin.Core.Solver.TimeSteps;

public interface INodeTimeStep
{
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

    public void Validate();
}