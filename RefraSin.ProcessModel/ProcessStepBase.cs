namespace RefraSin.ProcessModel;

public abstract class ProcessStepBase : IProcessStep
{
    /// <inheritdoc />
    public abstract ISystemState Solve(ISystemState inputState);

    /// <inheritdoc />
    public event EventHandler<IProcessStep.SystemStateReportedEventArgs>? SystemStateReported;

    /// <inheritdoc />
    public event EventHandler<IProcessStep.SystemChangeReportedEventArgs>? SystemChangeReported;

    protected virtual void ReportSystemState(IProcessStep.SystemStateReportedEventArgs reportedEventArgs)
    {
        SystemStateReported?.Invoke(this, reportedEventArgs);
    }

    protected virtual void ReportSystemState(ISystemState state)
    {
        ReportSystemState(new IProcessStep.SystemStateReportedEventArgs(this, state));
    }

    protected virtual void ReportSystemChange(IProcessStep.SystemChangeReportedEventArgs reportedEventArgs)
    {
        SystemChangeReported?.Invoke(this, reportedEventArgs);
    }

    protected virtual void ReportSystemChange(ISystemChange state)
    {
        ReportSystemChange(new IProcessStep.SystemChangeReportedEventArgs(this, state));
    }
}