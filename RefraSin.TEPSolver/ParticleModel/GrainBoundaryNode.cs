using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Oberflächenknoten, der Teil einer Korngrenze ist.
/// </summary>
public class GrainBoundaryNode : ContactNodeBase<GrainBoundaryNode>, IGrainBoundaryNode
{
    /// <inheritdoc />
    public GrainBoundaryNode(INode node, Particle particle, ISolverSession solverSession) : base(node, particle, solverSession) { }

    private GrainBoundaryNode(Guid id, double r, Angle phi, Particle particle, ISolverSession solverSession, Guid contactedNodeId,
        Guid contactedParticleId) : base(id, r, phi, particle,
        solverSession, contactedNodeId, contactedParticleId) { }

    /// <inheritdoc />
    public override NodeType Type => NodeType.GrainBoundary;

    /// <inheritdoc />
    public override ToUpperToLower<double> SurfaceEnergy => _surfaceEnergy ??= new ToUpperToLower<double>(
        InterfaceProperties.Energy,
        InterfaceProperties.Energy
    );

    private ToUpperToLower<double>? _surfaceEnergy;

    /// <inheritdoc />
    public override ToUpperToLower<double> SurfaceDiffusionCoefficient => _surfaceDiffusionCoefficient ??= new ToUpperToLower<double>(
        InterfaceProperties.DiffusionCoefficient,
        InterfaceProperties.DiffusionCoefficient
    );

    private ToUpperToLower<double>? _surfaceDiffusionCoefficient;

    /// <inheritdoc />
    public override NodeBase ApplyTimeStep(StepVector stepVector, double timeStepWidth, Particle particle)
    {
        var normalDisplacement = stepVector.NormalDisplacement(this) * timeStepWidth;
        var angle = SurfaceRadiusAngle.ToUpper + SurfaceNormalAngle.ToUpper;
        var newR = CosLaw.C(Coordinates.R, normalDisplacement, angle);
        var dPhi = SinLaw.Alpha(normalDisplacement, newR, angle);

        return new GrainBoundaryNode(Id, newR, Coordinates.Phi + dPhi, particle, SolverSession, ContactedNodeId, ContactedParticleId);
    }
}