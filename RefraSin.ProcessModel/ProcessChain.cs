namespace RefraSin.ProcessModel;

/// <summary>
/// Process step that chains multiple sub-steps together. Can be nested.
/// </summary>
public class ProcessChain : ProcessStepBase
{
    public ProcessChain(IEnumerable<IProcessStep> processSteps)
    {
        ProcessSteps = processSteps.ToList();
    }

    /// <inheritdoc />
    public override ISystemState<IParticle<IParticleNode>, IParticleNode> Solve(
        ISystemState<IParticle<IParticleNode>, IParticleNode> inputState
    )
    {
        var currentState = inputState;
        ReportSystemState(inputState);

        foreach (var processStep in ProcessSteps)
        {
            currentState = processStep.Solve(currentState);
            ReportSystemState(currentState);
        }

        return currentState;
    }

    /// <summary>
    /// List of sub-steps.
    /// </summary>
    public IList<IProcessStep> ProcessSteps { get; }
}
