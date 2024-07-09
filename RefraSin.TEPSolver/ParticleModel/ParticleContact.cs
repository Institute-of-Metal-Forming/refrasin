using RefraSin.Coordinates;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel;

public class ParticleContact : DirectedEdge<Particle>, IParticleContact
{
    private IList<ContactNodeBase>? _fromNodes;
    private IList<ContactNodeBase>? _toNodes;

    /// <inheritdoc />
    public ParticleContact(Guid id, Particle from, Particle to) : base(id, from, to)
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

    public IList<ContactNodeBase> FromNodes =>
        _fromNodes ??= From.Nodes.OfType<ContactNodeBase>().Where(n => n.ContactedParticleId == To.Id).ToArray();

    public IList<ContactNodeBase> ToNodes => _toNodes ??= FromNodes.Select(n => n.ContactedNode).ToArray();
}