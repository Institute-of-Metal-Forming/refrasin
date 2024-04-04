namespace RefraSin.ProcessModel;

public abstract class ProcessStepBase : IProcessStep
{
    /// <inheritdoc />
    public abstract ISystemState Solve(ISystemState inputState);

    /// <inheritdoc />
    public event EventHandler<IProcessStep.SystemStateReportedEventArgs>? SystemStateReported;

    /// <inheritdoc />
    public event EventHandler<IProcessStep.SystemStateTransitionReportedEventArgs>? SystemStateTransitionReported;

    protected virtual void ReportSystemState(IProcessStep.SystemStateReportedEventArgs reportedEventArgs)
    {
        SystemStateReported?.Invoke(this, reportedEventArgs);
    }

    protected virtual void ReportSystemState(ISystemState state)
    {
        ReportSystemState(new IProcessStep.SystemStateReportedEventArgs(this, state));
    }

    protected virtual void ReportSystemStateTransition(IProcessStep.SystemStateTransitionReportedEventArgs reportedEventArgs)
    {
        SystemStateTransitionReported?.Invoke(this, reportedEventArgs);
    }

    protected virtual void ReportSystemStateTransition(ISystemStateTransition state)
    {
        ReportSystemStateTransition(new IProcessStep.SystemStateTransitionReportedEventArgs(this, state));
    }
}