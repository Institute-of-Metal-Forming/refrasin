using RefraSin.ParticleModel;

namespace RefraSin.Storage;

/// <summary>
/// A immutable record of a solution step.
/// </summary>
public record SolutionStep : ISolutionStep
{
    public SolutionStep(double startTime, double endTime, IEnumerable<IParticleTimeStep> particleTimeSteps)
    {
        StartTime = startTime;
        EndTime = endTime;
        TimeStepWidth = endTime - startTime;
        ParticleTimeSteps = particleTimeSteps.Select(s => s as ParticleTimeStep ?? new ParticleTimeStep(s)).ToArray();
    }

    public SolutionStep(ISolutionStep step)
    {
        StartTime = step.StartTime;
        EndTime = step.EndTime;
        TimeStepWidth = StartTime - EndTime;
        ParticleTimeSteps = step.ParticleTimeSteps.Select(s => s as ParticleTimeStep ?? new ParticleTimeStep(s)).ToArray();
    }

    /// <inheritdoc />
    public double StartTime { get; }

    /// <inheritdoc />
    public double EndTime { get; }

    /// <inheritdoc />
    public double TimeStepWidth { get; }

    /// <inheritdoc cref="ISolutionStep.ParticleTimeSteps"/>
    public IReadOnlyList<ParticleTimeStep> ParticleTimeSteps { get; }

    IReadOnlyList<IParticleTimeStep> ISolutionStep.ParticleTimeSteps => ParticleTimeSteps;
}