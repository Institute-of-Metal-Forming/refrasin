using RefraSin.ParticleModel;

namespace RefraSin.Storage;

/// <summary>
/// Interface for types encapsulating data of a solution step.
/// </summary>
public interface ISolutionStep
{
    /// <summary>
    /// Time coordinate of the previous state.
    /// </summary>
    double StartTime { get; }

    /// <summary>
    /// Time coordinate of the next state.
    /// </summary>
    double EndTime { get; }

    /// <summary>
    /// Width of the time step. Shall generally equal <see cref="EndTime"/> - <see cref="StartTime"/>.
    /// </summary>
    double TimeStepWidth { get; }

    /// <summary>
    /// List of particle time steps included.
    /// </summary>
    IReadOnlyList<IParticleTimeStep> ParticleTimeSteps { get; }
}