using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel.Sintering;

public class SinteringStateTransition(
    ISystemState inputState,
    ISystemState outputState,
    IEnumerable<IParticleTimeStep> particleTimeSteps)
    : ISinteringStateStateTransition
{
    public ISystemState InputState { get; } = inputState;

    public ISystemState OutputState { get; } = outputState;

    /// <inheritdoc />
    public Guid InputStateId { get; } = inputState.Id;

    /// <inheritdoc />
    public Guid OutputStateId { get; } = outputState.Id;

    /// <inheritdoc />
    public double TimeStepWidth { get; } = outputState.Time - inputState.Time;

    /// <inheritdoc />
    public IReadOnlyList<IParticleTimeStep> ParticleTimeSteps { get; } = particleTimeSteps.ToArray();
}