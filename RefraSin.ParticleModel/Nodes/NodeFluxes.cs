using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Nodes;

/// <summary>
/// Record of fluxes on a node.
/// </summary>
public record NodeFluxes(
    Guid Id,
    Guid ParticleId,
    IPolarPoint Coordinates,
    NodeType Type,
    ToUpperToLower<double> InterfaceFlux,
    ToUpperToLower<double> VolumeFlux,
    double TransferFlux
) : Node(Id, ParticleId, Coordinates, Type), INodeFluxes { }
