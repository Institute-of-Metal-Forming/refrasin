using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public record ParticleContactEdge<TParticle>(
    TParticle From,
    TParticle To,
    IPolarVector ContactVector
) : IParticleContactEdge<TParticle>
    where TParticle : IParticle
{
    public ParticleContactEdge(TParticle from, TParticle to)
        : this(
            from,
            to,
            new PolarVector(from.Coordinates.Absolute.VectorTo(to.Coordinates.Absolute), from)
        ) { }

    public ParticleContactEdge(IEdge<TParticle> edge)
        : this(edge.From, edge.To) { }

    public ParticleContactEdge<TParticle> Reversed() =>
        new(To, From, new PolarVector(-ContactVector.Absolute, To));

    IEdge<TParticle> IEdge<TParticle>.Reversed() => Reversed();

    IEdge IEdge.Reversed() => Reversed();

    /// <inheritdoc />
    public double Distance => ContactVector.R;

    /// <inheritdoc />
    public Angle DirectionFrom => ContactVector.Phi;

    /// <inheritdoc />
    public Angle DirectionTo { get; } = new PolarVector(-ContactVector.Absolute, To).Phi;
}

public record ParticleContactEdge(
    Guid From,
    Guid To,
    double Distance,
    Angle DirectionFrom,
    Angle DirectionTo
) : IParticleContactEdge
{
    public ParticleContactEdge(
        IParticle from,
        IParticle to,
        double distance,
        Angle directionFrom,
        Angle directionTo
    )
        : this(from.Id, to.Id, distance, directionFrom, directionTo) { }

    public ParticleContactEdge(IEdge edge, double distance, Angle directionFrom, Angle directionTo)
        : this(edge.From, edge.To, distance, directionFrom, directionTo) { }

    public ParticleContactEdge Reversed() => new(To, From, Distance, DirectionTo, DirectionFrom);

    IEdge IEdge.Reversed() => Reversed();
}
