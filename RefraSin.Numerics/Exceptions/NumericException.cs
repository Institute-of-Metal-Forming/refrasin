namespace RefraSin.Numerics.Exceptions;

/// <summary>
/// A base class for exceptions occuring in numerical procedures.
/// </summary>
public class NumericException(string message, Exception? innerException = null)
    : Exception(message, innerException);
