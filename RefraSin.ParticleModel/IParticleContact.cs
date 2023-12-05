using RefraSin.Coordinates;
using RefraSin.Graphs;

namespace RefraSin.ParticleModel;

public interface IParticleContact : IEdge<IParticle>
{
    /// <summary>
    /// Distance between particle centers.
    /// </summary>
    public double Distance { get; }

    /// <summary>
    /// Direction angle in means of the <see cref="IParticleContact.From"/> particle.
    /// </summary>
    public Angle DirectionFrom { get; }

    /// <summary>
    /// Direction angle in means of the <see cref="IParticleContact.To"/> particle.
    /// </summary>
    public Angle DirectionTo { get; }
}