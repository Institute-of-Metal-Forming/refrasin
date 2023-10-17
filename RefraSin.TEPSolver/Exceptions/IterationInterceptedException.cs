namespace RefraSin.TEPSolver.Exceptions;

/// <summary>
/// Exception raised when an iteration procedure was intercepted.
/// </summary>
public abstract class IterationInterceptedException : NumericException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="loopLabel">label to identify the loop where the exception was raised</param>
    /// <param name="reason">the reason of interception</param>
    /// <param name="iterationCount">the count of iterations performed so far, values &lt; 0 are interpreted as missing information</param>
    /// <param name="innerException">optional inner exception object</param>
    /// <param name="furtherInformation">optional further information to include in the message</param>
    public IterationInterceptedException(string loopLabel, InterceptReason reason, int iterationCount = -1, Exception? innerException = null,
        string? furtherInformation = null)
        : base("", innerException)
    {
        LoopLabel = loopLabel;
        IterationCount = iterationCount;
        Reason = reason;

        var message = iterationCount >= 0
            ? $"The iteration loop '{loopLabel}' was intercepted at iteration {iterationCount} due to: {reason}"
            : $"The iteration loop '{loopLabel}' was intercepted due to: {reason}";

        if (!string.IsNullOrWhiteSpace(furtherInformation))
        {
            message += $" ({furtherInformation})";
        }

        Message = message;
    }

    /// <summary>
    /// Label to identify the loop where the exception was raised.
    /// </summary>
    public string LoopLabel { get; }

    /// <summary>
    /// The count of iterations performed so far, values &lt; 0 are meant as missing.
    /// </summary>
    public int IterationCount { get; }

    /// <summary>
    /// The reason of interception.
    /// </summary>
    public InterceptReason Reason { get; }

    /// <inheritdoc />
    public override string Message { get; }
}

/// <summary>
/// Represents the reason of interception.
/// </summary>
public enum InterceptReason
{
    /// <summary>
    /// Unspecific reason. Maybe look at <see cref="IterationInterceptedException.Message"/>.
    /// </summary>
    Unspecified,
    
    /// <summary>
    /// The specified maximum iteration count was exceeded.
    /// </summary>
    MaxIterationCountExceeded,

    /// <summary>
    /// An invalid state occured during iteration (maybe NaNs or values outside valid bounds).
    /// </summary>
    InvalidStateOccured,

    /// <summary>
    /// An exception was thrown during iteration. See <see cref="Exception.InnerException"/>.
    /// </summary>
    ExceptionOccured
}

/// <summary>
/// Derived of <see cref="IterationInterceptedException"/> representing a critical interception.
/// </summary>
public class CriticalIterationInterceptedException : IterationInterceptedException
{
    /// <inheritdoc />
    public CriticalIterationInterceptedException(string loopLabel, InterceptReason reason, int iterationCount = -1, Exception? innerException = null,
        string? furtherInformation = null) : base(loopLabel, reason, iterationCount, innerException, furtherInformation) { }
}

/// <summary>
/// Derived of <see cref="IterationInterceptedException"/> representing an uncritical interception.
/// </summary>
public class UncriticalIterationInterceptedException : IterationInterceptedException
{
    /// <inheritdoc />
    public UncriticalIterationInterceptedException(string loopLabel, InterceptReason reason, int iterationCount = -1,
        Exception? innerException = null, string? furtherInformation = null) : base(loopLabel, reason, iterationCount, innerException,
        furtherInformation) { }
}