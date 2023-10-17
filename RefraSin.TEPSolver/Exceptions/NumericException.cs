namespace RefraSin.TEPSolver.Exceptions;

/// <summary>
/// A base class for exceptions occuring in numerical procedures.
/// </summary>
public class NumericException : Exception
{
    public NumericException(string message, Exception? innerException = null) : base(message, innerException) { }
}