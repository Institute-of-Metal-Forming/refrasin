namespace RefraSin.ProcessModel;

public abstract class ProcessStepBase : IProcessStep
{
    /// <inheritdoc />
    public event EventHandler<IProcessStep.SystemStateReportedEventArgs>? SystemStateReported;

    /// <inheritdoc />
    public abstract ISystemState<IParticle<IParticleNode>, IParticleNode> Solve(ISystemState<IParticle<IParticleNode>, IParticleNode> inputState);

    public virtual void ReportSystemState(
        IProcessStep.SystemStateReportedEventArgs reportedEventArgs
    ) => SystemStateReported?.Invoke(this, reportedEventArgs);

    public virtual void ReportSystemState(ISystemState<IParticle<IParticleNode>, IParticleNode> state) =>
        ReportSystemState(new IProcessStep.SystemStateReportedEventArgs(this, state));
}
