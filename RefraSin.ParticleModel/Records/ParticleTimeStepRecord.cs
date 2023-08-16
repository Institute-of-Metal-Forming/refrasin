using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Records;

public record ParticleTimeStepRecord(Guid ParticleId, double RadialDisplacement, Angle AngleDisplacement, Angle RotationDisplacement,
    PolarVector DisplacementVector, double VolumeChange, IReadOnlyDictionary<Guid, NodeTimeStepRecord> NodeTimeSteps) : IParticleTimeStep
{
    public ParticleTimeStepRecord(IParticleTimeStep template) : this(
        template.ParticleId,
        template.RadialDisplacement,
        template.AngleDisplacement,
        template.RotationDisplacement,
        template.DisplacementVector,
        template.VolumeChange,
        template.NodeTimeSteps.Values.ToDictionary(ts => ts.NodeId, ts => new NodeTimeStepRecord(ts))
    ) { }

    /// <inheritdoc />
    public Guid ParticleId { get; } = ParticleId;

    /// <inheritdoc />
    public double RadialDisplacement { get; } = RadialDisplacement;

    /// <inheritdoc />
    public Angle AngleDisplacement { get; } = AngleDisplacement;

    /// <inheritdoc />
    public Angle RotationDisplacement { get; } = RotationDisplacement;

    /// <inheritdoc />
    public PolarVector DisplacementVector { get; } = DisplacementVector;

    /// <inheritdoc />
    public double VolumeChange { get; } = VolumeChange;

    /// <inheritdoc cref="IParticleTimeStep.NodeTimeSteps"/>
    public IReadOnlyDictionary<Guid, NodeTimeStepRecord> NodeTimeSteps { get; } = NodeTimeSteps;

    IReadOnlyDictionary<Guid, INodeTimeStep> IParticleTimeStep.NodeTimeSteps { get; } =
        NodeTimeSteps.Values.ToDictionary(ts => ts.NodeId, ts => (INodeTimeStep)ts);
}