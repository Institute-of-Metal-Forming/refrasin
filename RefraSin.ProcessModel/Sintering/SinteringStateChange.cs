using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel.Sintering;

public class SinteringStateChange(
    ISystemState inputState,
    ISystemState outputState,
    IEnumerable<IParticleTimeStep> particleTimeSteps)
    : ISinteringStateChange
{
    /// <inheritdoc />
    public ISystemState InputState { get; } = inputState;

    /// <inheritdoc />
    public ISystemState OutputState { get; } = outputState;

    /// <inheritdoc />
    public double TimeStepWidth { get; } = outputState.Time - inputState.Time;

    /// <inheritdoc />
    public IReadOnlyList<IParticleTimeStep> ParticleTimeSteps { get; } = particleTimeSteps.ToArray();
}