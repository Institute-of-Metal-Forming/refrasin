using MathNet.Numerics.LinearAlgebra;

namespace RefraSin.Numerics.LinearSolvers;

public interface ILinearSolver
{
    Vector<double> Solve(Matrix<double> matrix, Vector<double> rightSide);
}
