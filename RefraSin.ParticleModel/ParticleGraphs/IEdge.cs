using RefraSin.Coordinates;

namespace RefraSin.ParticleModel.ParticleGraphs;

public interface IEdge
{
    Guid Start { get; }
    Guid End { get; }

    Angle Angle { get; }
}