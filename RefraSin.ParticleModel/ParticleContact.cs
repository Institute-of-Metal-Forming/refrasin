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
    public ParticleContact(IParticle from, IParticle to, double distance, Angle directionFrom, Angle directionTo) : base(from, to)
    {
        Distance = distance;
        DirectionFrom = directionFrom;
        DirectionTo = directionTo;
    }

    public ParticleContact(IParticleContact template) : base(template.From, template.To)
    {
        Distance = template.Distance;
        DirectionFrom = template.DirectionFrom;
        DirectionTo = template.DirectionTo;
    }

    /// <inheritdoc />
    public double Distance { get; }

    /// <inheritdoc />
    public Angle DirectionFrom { get; }

    /// <inheritdoc />
    public Angle DirectionTo { get; }
}