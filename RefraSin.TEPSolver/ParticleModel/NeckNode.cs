using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Oberflächenknoten welcher am Punkt des Sinterhalse liegt. Vermittelt den Übergang zwischen freien Oberflächen und Korngrenzen.
/// </summary>
public class NeckNode : ContactNodeBase<NeckNode>, INeckNode
{
    /// <inheritdoc />
    public NeckNode(INode node, Particle particle, ISolverSession solverSession) : base(node, particle, solverSession) { }

    private NeckNode(Guid id, double r, Angle phi, Particle particle, ISolverSession solverSession, Guid contactedNodeId,
        Guid contactedParticleId) : base(id, r, phi, particle,
        solverSession, contactedNodeId, contactedParticleId) { }

    /// <inheritdoc />
    public override NodeType Type => NodeType.NeckNode;

    public override ToUpperToLower<double> SurfaceEnergy => _surfaceEnergy ??= new ToUpperToLower<double>(
        Upper.SurfaceEnergy.ToLower,
        Lower.SurfaceEnergy.ToUpper
    );

    private ToUpperToLower<double>? _surfaceEnergy;

    /// <inheritdoc />
    public override ToUpperToLower<double> SurfaceDiffusionCoefficient => _surfaceDiffusionCoefficient ??= new ToUpperToLower<double>(
        Upper.SurfaceDiffusionCoefficient.ToLower,
        Lower.SurfaceDiffusionCoefficient.ToUpper
    );

    private ToUpperToLower<double>? _surfaceDiffusionCoefficient;

    /// <inheritdoc />
    public override NodeBase ApplyTimeStep(StepVector stepVector, double timeStepWidth, Particle particle)
    {
        var normalDisplacement = stepVector[this].NormalDisplacement * timeStepWidth;
        var angle = SurfaceRadiusAngle.ToUpper + SurfaceVectorAngle.Normal;
        var newR = CosLaw.C(Coordinates.R, normalDisplacement, angle);
        var dPhi = SinLaw.Alpha(normalDisplacement, newR, angle);

        return new NeckNode(Id, newR, Coordinates.Phi + dPhi, particle, SolverSession, ContactedNodeId, ContactedParticleId);
    }
}