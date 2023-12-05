namespace RefraSin.TEPSolver.Exceptions;

public class StepRejectedException : NumericException
{
    /// <inheritdoc />
    public StepRejectedException() : base("Step was rejected due to insufficient precision.") { }
}