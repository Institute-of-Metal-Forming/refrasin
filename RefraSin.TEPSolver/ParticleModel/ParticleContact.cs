using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel;

public class ParticleContact : DirectedEdge<IParticle>, IParticleContact
{
    /// <inheritdoc />
    public ParticleContact(IEdge<Particle> edge) : this(edge.From, edge.To) { }

    /// <inheritdoc />
    public ParticleContact(Particle from, Particle to) : base(from, to)
    {
        Distance = from.CenterCoordinates.DistanceTo(to.CenterCoordinates);
        DirectionFrom = new PolarVector(to.CenterCoordinates - from.CenterCoordinates, from.LocalCoordinateSystem).Phi;
        DirectionTo = new PolarVector(from.CenterCoordinates - to.CenterCoordinates, to.LocalCoordinateSystem).Phi;
    }

    /// <inheritdoc />
    public double Distance { get; }

    /// <inheritdoc />
    public Angle DirectionFrom { get; }

    /// <inheritdoc />
    public Angle DirectionTo { get; }
}