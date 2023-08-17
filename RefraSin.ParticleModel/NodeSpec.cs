using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel;

public record NodeSpec(Guid Id, Guid ParticleId, PolarPoint Coordinates) : INodeSpec
{
    /// <inheritdoc />
    public Guid Id { get; } = Id;

    /// <inheritdoc />
    public Guid ParticleId { get; } = ParticleId;

    /// <inheritdoc />
    public PolarPoint Coordinates { get; } = Coordinates;
}