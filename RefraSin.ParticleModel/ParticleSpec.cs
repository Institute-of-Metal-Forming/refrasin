using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;

namespace RefraSin.ParticleModel;

public record ParticleSpec(Guid Id, AbsolutePoint AbsoluteCenterCoordinates, Angle RotationAngle, Guid MaterialId, IReadOnlyList<INodeSpec> NodeSpecs)
    : IParticleSpec
{
    /// <inheritdoc />
    public Guid Id { get; } = Id;

    /// <inheritdoc />
    public AbsolutePoint AbsoluteCenterCoordinates { get; } = AbsoluteCenterCoordinates;

    /// <inheritdoc />
    public Angle RotationAngle { get; } = RotationAngle;

    /// <inheritdoc />
    public Guid MaterialId { get; } = MaterialId;

    /// <inheritdoc />
    public IReadOnlyList<INodeSpec> NodeSpecs { get; } = NodeSpecs;

    /// <inheritdoc />
    public INodeSpec this[int i] => i >= 0 ? NodeSpecs[(i % NodeSpecs.Count)] : NodeSpecs[^-(i % NodeSpecs.Count)];

    /// <inheritdoc />
    public INodeSpec this[Guid nodeId] => NodeSpecs.FirstOrDefault(n => n.Id == nodeId) ??
                                          throw new IndexOutOfRangeException($"A node with ID {nodeId} is not present in this particle.");
}