using RefraSin.Coordinates;

namespace RefraSin.ParticleModel;

public record ParticleTimeStep : IParticleTimeStep
{
    public ParticleTimeStep(Guid particleId, double radialDisplacement, Angle angleDisplacement, Angle rotationDisplacement,
        IEnumerable<INodeTimeStep> nodeTimeSteps)
    {
        ParticleId = particleId;
        RadialDisplacement = radialDisplacement;
        AngleDisplacement = angleDisplacement;
        RotationDisplacement = rotationDisplacement;
        NodeTimeSteps = nodeTimeSteps.Select(ns => ns as NodeTimeStep ?? new NodeTimeStep(ns)).ToDictionary(ns => ns.NodeId);
    }

    public ParticleTimeStep(IParticleTimeStep template) : this(
        template.ParticleId,
        template.RadialDisplacement,
        template.AngleDisplacement,
        template.RotationDisplacement,
        template.NodeTimeSteps.Values
    ) { }

    /// <inheritdoc />
    public Guid ParticleId { get; }

    /// <inheritdoc />
    public double RadialDisplacement { get; }

    /// <inheritdoc />
    public Angle AngleDisplacement { get; }

    /// <inheritdoc />
    public Angle RotationDisplacement { get; }

    /// <inheritdoc cref="IParticleTimeStep.NodeTimeSteps"/>
    public IReadOnlyDictionary<Guid, NodeTimeStep> NodeTimeSteps { get; }

    IReadOnlyDictionary<Guid, INodeTimeStep> IParticleTimeStep.NodeTimeSteps =>
        NodeTimeSteps.Values.ToDictionary(ts => ts.NodeId, ts => (INodeTimeStep)ts);
}