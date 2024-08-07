using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Nodes;

/// <summary>
/// Record of node shifts.
/// </summary>
public record NodeShifts(
    Guid Id,
    Guid ParticleId,
    PolarPoint Coordinates,
    NodeType Type,
    NormalTangential<double> Shift
) : Node(Id, ParticleId, Coordinates, Type), INodeShifts { }
