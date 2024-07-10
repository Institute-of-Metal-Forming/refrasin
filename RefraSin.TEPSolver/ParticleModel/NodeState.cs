using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.TEPSolver.ParticleModel;

internal record NodeState(
    Guid Id,
    Guid ParticleId,
    IPolarPoint Coordinates,
    NodeType Type,
    ToUpperToLower<double> SurfaceDistance,
    ToUpperToLower<Angle> SurfaceRadiusAngle,
    ToUpperToLower<Angle> AngleDistance,
    ToUpperToLower<double> Volume,
    ToUpperToLower<Angle> SurfaceNormalAngle,
    ToUpperToLower<Angle> SurfaceTangentAngle,
    ToUpperToLower<double> InterfaceFlux,
    ToUpperToLower<double> VolumeFlux,
    double TransferFlux,
    NormalTangential<double> GibbsEnergyGradient,
    NormalTangential<double> VolumeGradient,
    NormalTangential<double> Shift
) : INodeGeometry, INodeGradients, INodeShifts, INodeFluxes;

internal record ContactNodeState(
    Guid Id,
    Guid ParticleId,
    IPolarPoint Coordinates,
    NodeType Type,
    ToUpperToLower<double> SurfaceDistance,
    ToUpperToLower<Angle> SurfaceRadiusAngle,
    ToUpperToLower<Angle> AngleDistance,
    ToUpperToLower<double> Volume,
    ToUpperToLower<Angle> SurfaceNormalAngle,
    ToUpperToLower<Angle> SurfaceTangentAngle,
    Guid ContactedNodeId,
    Guid ContactedParticleId,
    Angle AngleDistanceToContactDirection,
    IPolarPoint ContactedParticlesCenter,
    NormalTangential<Angle> CenterShiftVectorDirection,
    NormalTangential<double> ContactDistanceGradient,
    NormalTangential<double> ContactDirectionGradient,
    ToUpperToLower<double> InterfaceFlux,
    ToUpperToLower<double> VolumeFlux,
    double TransferFlux,
    NormalTangential<double> GibbsEnergyGradient,
    NormalTangential<double> VolumeGradient,
    NormalTangential<double> Shift
) : INodeContactGeometry, INodeContactGradients, INodeShifts, INodeFluxes;
