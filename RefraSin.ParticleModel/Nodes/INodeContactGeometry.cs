using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Nodes;

public interface INodeContactGeometry : INodeContact
{
    Angle AngleDistanceToContactDirection { get; }

    IPolarVector ContactVector { get; }
}