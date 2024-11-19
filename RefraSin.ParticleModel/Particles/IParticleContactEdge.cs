using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public interface IParticleContactEdge<out TParticle> : IParticleContactEdge, IEdge<TParticle>
    where TParticle : IParticle
{
    IPolarVector ContactVector { get; }
}

public interface IParticleContactEdge : IEdge
{
    double Distance { get; }

    Angle DirectionFrom { get; }

    Angle DirectionTo { get; }
}
