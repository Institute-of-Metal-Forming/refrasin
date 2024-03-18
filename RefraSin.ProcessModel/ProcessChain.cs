using System.Collections;

namespace RefraSin.ProcessModel;

/// <summary>
/// Process step that chains multiple sub-steps together. Can be nested.
/// </summary>
public class ProcessChain : IProcessStep
{
    public ProcessChain(IEnumerable<IProcessStep> processSteps)
    {
        ProcessSteps = processSteps.ToList();
    }

    /// <inheritdoc />
    public ISystemState Solve(ISystemState inputState)
    {
        var currentState = inputState;
        
        foreach (var processStep in ProcessSteps)
        {
            currentState = processStep.Solve(currentState);
        }

        return currentState;
    }

    /// <summary>
    /// List of sub-steps.
    /// </summary>
    public IList<IProcessStep> ProcessSteps { get; }
}