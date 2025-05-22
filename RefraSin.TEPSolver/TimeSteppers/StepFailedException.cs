using RefraSin.Numerics.Exceptions;

namespace RefraSin.TEPSolver.TimeSteppers;

public class StepFailedException(
    string message = "Calculation of time step failed.",
    Exception? innerException = null
) : NumericException(message, innerException) { }
