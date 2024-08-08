using RefraSin.Coordinates.Absolute;

namespace RefraSin.Coordinates;

/// <summary>
///     Schnittstelle für Vektoren.
/// </summary>
public interface IVector : ICoordinates, IVectorOperations<IVector>
{
    /// <summary>
    ///     Gets the absolute coordinate representation of this vector.
    /// </summary>
    public AbsoluteVector Absolute { get; }

    /// <summary>
    ///     Gets the euclidean norm of this vector.
    /// </summary>
    public double Norm { get; }

    /// <inheritdoc />
    IVector IVectorOperations<IVector>.Add(IVector v) => Absolute.Add(v.Absolute);

    /// <inheritdoc />
    IVector IVectorOperations<IVector>.Reverse() => Absolute.Reverse();

    /// <inheritdoc />
    double IVectorOperations<IVector>.ScalarProduct(IVector v) =>
        Absolute.ScalarProduct(v.Absolute);
}
