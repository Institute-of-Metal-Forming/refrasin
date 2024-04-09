namespace RefraSin.ProcessModel;

public abstract class ProcessStepBase : IProcessStep
{
    /// <inheritdoc />
    public event EventHandler<IProcessStep.SystemStateReportedEventArgs>? SystemStateReported;

    /// <inheritdoc />
    public event EventHandler<IProcessStep.SystemStateTransitionReportedEventArgs>? SystemStateTransitionReported;

    /// <inheritdoc />
    public abstract ISystemState Solve(ISystemState inputState);

    public virtual void ReportSystemState(
        IProcessStep.SystemStateReportedEventArgs reportedEventArgs
    ) => SystemStateReported?.Invoke(this, reportedEventArgs);

    public virtual void ReportSystemState(ISystemState state) =>
        ReportSystemState(new IProcessStep.SystemStateReportedEventArgs(this, state));

    public virtual void ReportSystemStateTransition(
        IProcessStep.SystemStateTransitionReportedEventArgs reportedEventArgs
    ) => SystemStateTransitionReported?.Invoke(this, reportedEventArgs);

    public virtual void ReportSystemStateTransition(ISystemStateTransition state) =>
        ReportSystemStateTransition(
            new IProcessStep.SystemStateTransitionReportedEventArgs(this, state)
        );
}
