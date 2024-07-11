using RefraSin.Coordinates;
using RefraSin.Graphs;

namespace RefraSin.ParticleModel.Particles;

public class ParticleContact : UndirectedEdge<IParticle>, IParticleContact
{
    /// <inheritdoc />
    public ParticleContact(Guid id, IParticle from, IParticle to, double distance, Angle directionFrom, Angle directionTo) : base(id, from, to)
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
    
    /// <inheritdoc />
    public ParticleContact(IEdge<IParticle> edge, double distance, Angle directionFrom, Angle directionTo) : base(edge)
    {
        Distance = distance;
        DirectionFrom = directionFrom;
        DirectionTo = directionTo;
    }

    public ParticleContact(IParticleContact template) : base(template)
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