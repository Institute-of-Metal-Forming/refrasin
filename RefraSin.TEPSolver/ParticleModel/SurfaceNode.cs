using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Oberflächenknoten, der Teil einer freien Oberfläche ist.
/// </summary>
public class SurfaceNode : NodeBase, ISurfaceNode
{
    /// <inheritdoc />
    public SurfaceNode(INode node, Particle particle, ISolverSession solverSession) : base(node, particle, solverSession) { }

    private SurfaceNode(Guid id, double r, Angle phi, Particle particle, ISolverSession solverSession) : base(id, r, phi, particle, solverSession) { }

    /// <inheritdoc />
    public override NodeType Type => NodeType.Surface;

    /// <inheritdoc />
    public override ToUpperToLower<double> InterfaceEnergy => _interfaceEnergy ??= new ToUpperToLower<double>(
        Particle.SurfaceProperties.Energy,
        Particle.SurfaceProperties.Energy
    );

    private ToUpperToLower<double>? _interfaceEnergy;

    /// <inheritdoc />
    public override ToUpperToLower<double> InterfaceDiffusionCoefficient => _interfaceDiffusionCoefficient ??= new ToUpperToLower<double>(
        Particle.SurfaceProperties.DiffusionCoefficient,
        Particle.SurfaceProperties.DiffusionCoefficient
    );

    private ToUpperToLower<double>? _interfaceDiffusionCoefficient;

    /// <inheritdoc />
    public override NodeBase ApplyTimeStep(StepVector stepVector, double timeStepWidth, Particle particle)
    {
        var normalDisplacement = stepVector.NormalDisplacement(this) * timeStepWidth;
        var angle = SurfaceRadiusAngle.ToUpper + SurfaceNormalAngle.ToUpper;
        var newR = CosLaw.C(Coordinates.R, normalDisplacement, angle);
        var dPhi = SinLaw.Alpha(normalDisplacement, newR, angle);

        return new SurfaceNode(Id, newR, Coordinates.Phi + dPhi, particle, SolverSession);
    }
}