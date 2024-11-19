namespace RefraSin.TEPSolver.Recovery;

public class RecoveryFailedException : InvalidOperationException
{
    public RecoveryFailedException(IStateRecoverer source, Exception? innerException)
        : base("Recovery failed.", innerException)
    {
        Source = source;
    }

    public RecoveryFailedException(
        IStateRecoverer source,
        string message,
        Exception? innerException
    )
        : base("Recovery failed: " + message, innerException)
    {
        Source = source;
    }

    public IStateRecoverer Source { get; }
}
