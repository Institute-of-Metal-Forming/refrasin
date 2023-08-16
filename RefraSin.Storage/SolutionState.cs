using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Records;

namespace RefraSin.Storage;

/// <summary>
/// A immutable record of a solution state.
/// </summary>
public record SolutionState : ISolutionState
{
    public SolutionState(double time, IEnumerable<IParticle> particleStates)
    {
        Time = time;
        ParticleStates = particleStates.Select(s => s as ParticleRecord ?? new ParticleRecord(s)).ToArray();
    }

    public SolutionState(ISolutionState state)
    {
        Time = state.Time;
        ParticleStates = state.ParticleStates.Select(s => s as ParticleRecord ?? new ParticleRecord(s)).ToArray();
    }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc cref="ISolutionState.ParticleStates"/>
    public IReadOnlyList<ParticleRecord> ParticleStates { get; }

    IReadOnlyList<IParticle> ISolutionState.ParticleStates => ParticleStates;
}