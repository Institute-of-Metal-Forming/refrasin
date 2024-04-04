using System.Collections;

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
    public override ISystemState Solve(ISystemState inputState)
    {
        var currentState = inputState;
        ReportSystemState(inputState);

        foreach (var processStep in ProcessSteps)
        {
            processStep.SystemStateReported += HandleSubStepSystemStateReported;
            processStep.SystemStateTransitionReported += HandleSubStepSystemChangeReported;
            
            currentState = processStep.Solve(currentState);
            
            processStep.SystemStateReported -= HandleSubStepSystemStateReported;
            processStep.SystemStateTransitionReported -= HandleSubStepSystemChangeReported;
            
            ReportSystemState(currentState);
        }

        return currentState;
    }

    private void HandleSubStepSystemStateReported(object? sender, IProcessStep.SystemStateReportedEventArgs eventArgs)
    {
        ReportSystemState(eventArgs);
    }

    private void HandleSubStepSystemChangeReported(object? sender, IProcessStep.SystemStateTransitionReportedEventArgs eventArgs)
    {
        ReportSystemStateTransition(eventArgs);
    }

    /// <summary>
    /// List of sub-steps.
    /// </summary>
    public IList<IProcessStep> ProcessSteps { get; }
}