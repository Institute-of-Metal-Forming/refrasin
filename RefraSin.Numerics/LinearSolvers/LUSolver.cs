using MathNet.Numerics.LinearAlgebra;

namespace RefraSin.Numerics.LinearSolvers;

public class LUSolver : ILinearSolver
{
    /// <inheritdoc />
    public Vector<double> Solve(Matrix<double> matrix, Vector<double> rightSide) => matrix.LU().Solve(rightSide);
}