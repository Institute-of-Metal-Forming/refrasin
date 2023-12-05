using RefraSin.Coordinates;
using RefraSin.Graphs;

namespace RefraSin.ParticleModel;

public class ParticleContact : UndirectedEdge<IParticle>, IParticleContact
{
    /// <inheritdoc />
    public ParticleContact(IEdge<IParticle> edge, double distance, Angle directionFrom, Angle directionTo) : base(edge)
    {
        Distance = distance;
        DirectionFrom = directionFrom;
        DirectionTo = directionTo;
    }

    /// <inheritdoc />
    public ParticleContact(IParticle start, IParticle end, double distance, Angle directionFrom, Angle directionTo) : base(start, end)
    {
        Distance = distance;
        DirectionFrom = directionFrom;
        DirectionTo = directionTo;
    }

    /// <inheritdoc />
    public double Distance { get; }

    /// <inheritdoc />
    public Angle DirectionFrom { get; }

    /// <inheritdoc />
    public Angle DirectionTo { get; }
}