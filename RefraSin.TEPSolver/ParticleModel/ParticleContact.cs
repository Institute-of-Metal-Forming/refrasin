using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel;

public class ParticleContact : DirectedEdge<Particle>, IParticleContact
{
    /// <inheritdoc />
    public ParticleContact(IEdge<Particle> edge) : this(edge.From, edge.To) { }

    /// <inheritdoc />
    public ParticleContact(Particle from, Particle to) : base(from, to)
    {
        Distance = from.CenterCoordinates.DistanceTo(to.CenterCoordinates);
        DirectionFrom = new PolarVector(to.CenterCoordinates - from.CenterCoordinates, from.LocalCoordinateSystem).Phi;
        DirectionTo = new PolarVector(from.CenterCoordinates - to.CenterCoordinates, to.LocalCoordinateSystem).Phi;

        FromNodes = from.Nodes.OfType<ContactNodeBase>().Where(n=> n.ContactedParticleId == To.Id).ToArray();
        ToNodes = FromNodes.Select(n => n.ContactedNode).ToArray();
    }

    /// <inheritdoc />
    public double Distance { get; }

    /// <inheritdoc />
    public Angle DirectionFrom { get; }

    /// <inheritdoc />
    public Angle DirectionTo { get; }

    /// <inheritdoc />
    public bool Equals(IEdge<IParticle>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return From.Equals(other.From) && To.Equals(other.To);
    }

    /// <inheritdoc />
    IParticle IEdge<IParticle>.From => From;

    /// <inheritdoc />
    IParticle IEdge<IParticle>.To => To;

    /// <inheritdoc />
    public bool IsEdgeFrom(IParticle from) => From.Equals(from);

    /// <inheritdoc />
    public bool IsEdgeTo(IParticle to) => To.Equals(to);

    /// <inheritdoc />
    IEdge<IParticle> IEdge<IParticle>.Reversed() => new DirectedEdge<IParticle>(To, From);
    
    public IList<ContactNodeBase> FromNodes { get; }
    
    public IList<ContactNodeBase> ToNodes { get; }
}