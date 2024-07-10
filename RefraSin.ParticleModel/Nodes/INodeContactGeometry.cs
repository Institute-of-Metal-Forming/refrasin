using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Nodes;

public interface INodeContactGeometry : INodeContactNeighbors, INodeGeometry
{
    Angle AngleDistanceToContactDirection => Coordinates.AngleTo(ContactedParticlesCenter, true);

    PolarPoint ContactedParticlesCenter => new(ContactedParticle.Coordinates, Particle);
}