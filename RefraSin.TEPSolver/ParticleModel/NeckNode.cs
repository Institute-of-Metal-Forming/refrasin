using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.TEPSolver.StepVectors;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Oberflächenknoten welcher am Punkt des Sinterhalse liegt. Vermittelt den Übergang zwischen freien Oberflächen und Korngrenzen.
/// </summary>
public class NeckNode : ContactNodeBase<NeckNode>
{
    /// <inheritdoc />
    public NeckNode(INode node, Particle particle)
        : base(node, particle) { }

    private NeckNode(
        Guid id,
        double r,
        Angle phi,
        Particle particle,
        Guid contactedNodeId,
        Guid contactedParticleId
    )
        : base(id, r, phi, particle, contactedNodeId, contactedParticleId) { }

    /// <inheritdoc />
    public override NodeType Type => NodeType.Neck;

    public override ToUpperToLower<double> InterfaceEnergy =>
        _interfaceEnergy ??= new ToUpperToLower<double>(
            Upper.InterfaceEnergy.ToLower,
            Lower.InterfaceEnergy.ToUpper
        );

    private ToUpperToLower<double>? _interfaceEnergy;

    /// <inheritdoc />
    public override ToUpperToLower<double> InterfaceDiffusionCoefficient =>
        _interfaceDiffusionCoefficient ??= new ToUpperToLower<double>(
            Upper.InterfaceDiffusionCoefficient.ToLower,
            Lower.InterfaceDiffusionCoefficient.ToUpper
        );

    private ToUpperToLower<double>? _interfaceDiffusionCoefficient;

    /// <inheritdoc />
    public override NodeBase ApplyTimeStep(
        StepVector stepVector,
        double timeStepWidth,
        Particle particle
    )
    {
        var normalDisplacement = new PolarVector(
            SurfaceRadiusAngle.ToUpper + SurfaceNormalAngle.ToUpper,
            stepVector.NormalDisplacement(this) * timeStepWidth
        );
        var tangentialDisplacement = new PolarVector(
            SurfaceRadiusAngle.ToUpper + SurfaceTangentAngle.ToUpper,
            stepVector.TangentialDisplacement(this) * timeStepWidth
        );
        var totalDisplacement = normalDisplacement + tangentialDisplacement;

        var newR = CosLaw.C(Coordinates.R, totalDisplacement.R, totalDisplacement.Phi);
        var dPhi = SinLaw.Alpha(totalDisplacement.R, newR, totalDisplacement.Phi);

        return new NeckNode(
            Id,
            newR,
            Coordinates.Phi + dPhi,
            particle,
            ContactedNodeId,
            ContactedParticleId
        );
    }
}
