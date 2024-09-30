using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Logging;

namespace RefraSin.Numerics.RootFinding;

public interface IRootFinder
{
    public Vector<double> FindRoot(
        Func<Vector<double>, Vector<double>> function,
        Func<Vector<double>, Matrix<double>> jacobian,
        Vector<double> initialGuess,
        ILogger? logger = null
    );
}
