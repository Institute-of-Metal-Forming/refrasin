using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel.Sintering;

public interface ISinteringStateChange : ISystemChange
{
    /// <summary>
    /// Width of the time step.
    /// </summary>
    double TimeStepWidth { get; }

    /// <summary>
    /// List of particle time steps included.
    /// </summary>
    IReadOnlyList<IParticleTimeStep> ParticleTimeSteps { get; }
}