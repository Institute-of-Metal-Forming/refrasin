namespace RefraSin.Coordinates;

/// <summary>
///     Exception, that is thrown, if an operation is invalid due to differences in the coordinate systems of the operands.
/// </summary>
/// <remarks>
///     Most operations involving two points or vectors are only defined, if they are in the same coordinate system.
///     This was decided to avoid hard to find errors in fallback on absolute coordinates.
///     If one needs to let these operations work on instances with different systems, use IVector.
///     <see cref="IVector.Absolute" /> and IPoint.<see cref="IPoint.Absolute" /> explicitly or cast the coordinates to the
///     same system by use of the copy constructors.
/// </remarks>
public class DifferentCoordinateSystemException : InvalidOperationException
{
    /// <summary>
    ///     Creates a new exception, that is thrown, if an operation is invalid due to differences in the coordinate systems of
    ///     the operands.
    /// </summary>
    /// <param name="first">first of the involved coordinates</param>
    /// <param name="second">second of the involved coordinates</param>
    public DifferentCoordinateSystemException(ICoordinates first, ICoordinates second)
        : base("Both instances must share the same coordinate system.")
    {
        First = first;
        Second = second;
    }

    /// <summary>
    ///     Gets the first of the involved coordinates.
    /// </summary>
    public ICoordinates First { get; }

    /// <summary>
    ///     Gets the first of the involved coordinates.
    /// </summary>
    public ICoordinates Second { get; }
}
