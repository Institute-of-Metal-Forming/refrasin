using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Oberflächenknoten, der Teil einer Korngrenze ist.
/// </summary>
internal class GrainBoundaryNode : ContactNode<GrainBoundaryNode>, IGrainBoundaryNode
{
    /// <inheritdoc />
    public GrainBoundaryNode(INode node, Particle particle, ISolverSession solverSession) : base(node, particle, solverSession) { }

    /// <inheritdoc />
    public override ToUpperToLower SurfaceEnergy => _surfaceEnergy ??= new ToUpperToLower(
        MaterialInterface.InterfaceEnergy,
        MaterialInterface.InterfaceEnergy
    );

    private ToUpperToLower? _surfaceEnergy;

    /// <inheritdoc />
    public override ToUpperToLower SurfaceDiffusionCoefficient => _surfaceDiffusionCoefficient ??= new ToUpperToLower(
        MaterialInterface.DiffusionCoefficient,
        MaterialInterface.DiffusionCoefficient
    );

    private ToUpperToLower? _surfaceDiffusionCoefficient;

    /// <inheritdoc />
    protected override void ClearCaches()
    {
        base.ClearCaches();
        _surfaceEnergy = null;
        _surfaceDiffusionCoefficient = null;
    }

    /// <inheritdoc />
    protected override void CheckState(INode state)
    {
        base.CheckState(state);

        if (state is not IGrainBoundaryNode)
            throw new ArgumentException($"The given state is no instance of {nameof(IGrainBoundaryNode)}", nameof(state));
    }
}