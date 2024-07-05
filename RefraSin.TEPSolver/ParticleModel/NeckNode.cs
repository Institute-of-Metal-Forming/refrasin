using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static RefraSin.Coordinates.Constants;

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
    public override NodeType Type => NodeType.Neck;

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
    public override ToUpperToLower<Angle> SurfaceNormalAngle => _surfaceNormalAngle ??= Upper is GrainBoundaryNode
        ? new ToUpperToLower<Angle>(HalfOfPi, ThreeHalfsOfPi - SurfaceRadiusAngle.ToUpper - SurfaceRadiusAngle.ToLower)
        : new ToUpperToLower<Angle>(ThreeHalfsOfPi - SurfaceRadiusAngle.ToUpper - SurfaceRadiusAngle.ToLower, HalfOfPi);

    /// <inheritdoc />
    public override ToUpperToLower<Angle> SurfaceTangentAngle => _surfaceTangentAngle ??= Upper is GrainBoundaryNode
        ? new ToUpperToLower<Angle>(0, Pi - SurfaceRadiusAngle.ToUpper - SurfaceRadiusAngle.ToLower)
        : new ToUpperToLower<Angle>(Pi - SurfaceRadiusAngle.ToUpper - SurfaceRadiusAngle.ToLower, 0);

    /// <inheritdoc />
    public override NodeBase ApplyTimeStep(StepVector stepVector, double timeStepWidth, Particle particle)
    {
        var normalDisplacement = new PolarVector(SurfaceRadiusAngle.ToUpper + SurfaceNormalAngle.ToUpper,
            stepVector.NormalDisplacement(this) * timeStepWidth);
        var tangentialDisplacement = new PolarVector(SurfaceRadiusAngle.ToUpper + SurfaceTangentAngle.ToUpper,
            stepVector.TangentialDisplacement(this) * timeStepWidth);
        var totalDisplacement = normalDisplacement + tangentialDisplacement;

        var newR = CosLaw.C(Coordinates.R, totalDisplacement.R, totalDisplacement.Phi);
        var dPhi = SinLaw.Alpha(totalDisplacement.R, newR, totalDisplacement.Phi);

        return new NeckNode(Id, newR, Coordinates.Phi + dPhi, particle, SolverSession, ContactedNodeId, ContactedParticleId);
    }
}