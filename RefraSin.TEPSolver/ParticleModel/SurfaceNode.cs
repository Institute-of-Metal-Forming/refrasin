using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Oberflächenknoten, der Teil einer freien Oberfläche ist.
/// </summary>
public class SurfaceNode : Node, ISurfaceNode
{
    /// <inheritdoc />
    public SurfaceNode(INode node, Particle particle, ISolverSession solverSession) : base(node, particle, solverSession) { }

    /// <inheritdoc />
    public override ToUpperToLower SurfaceEnergy => _surfaceEnergy ??= new ToUpperToLower(
        Particle.Material.SurfaceEnergy,
        Particle.Material.SurfaceEnergy
    );

    private ToUpperToLower? _surfaceEnergy;

    /// <inheritdoc />
    public override ToUpperToLower SurfaceDiffusionCoefficient => _surfaceDiffusionCoefficient ??= new ToUpperToLower(
        Particle.Material.SurfaceDiffusionCoefficient,
        Particle.Material.SurfaceDiffusionCoefficient
    );

    /// <inheritdoc />
    public override double TransferCoefficient => 0;

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

        if (state is not ISurfaceNode)
            throw new ArgumentException($"The given state is no instance of {nameof(ISurfaceNode)}", nameof(state));
    }
}