using RefraSin.MaterialData;

namespace RefraSin.ProcessModel.Sintering;

public interface ISinteringStep : IProcessStep, ISinteringConditions
{
    /// <summary>
    /// The solver used to solve this step.
    /// </summary>
    public IProcessStepSolver<ISinteringStep> Solver { get; }

    public IReadOnlyList<IMaterial> Materials { get; }
}