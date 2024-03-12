using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.LinearAlgebra.Solvers;

namespace RefraSin.Numerics.LinearSolvers;

public class IterativeSolver(IIterativeSolver<double> solver, Iterator<double> iterator, IPreconditioner<double> preconditioner)
    : ILinearSolver
{
    public IIterativeSolver<double> Solver { get; } = solver;

    public Iterator<double> Iterator { get; } = iterator;

    public IPreconditioner<double> Preconditioner { get; } = preconditioner;

    /// <inheritdoc />
    public Vector<double> Solve(Matrix<double> matrix, Vector<double> rightSide)
    {
        Iterator.Reset();
        var sol = Vector<double>.Build.Dense(rightSide.Count);
        Solver.Solve(matrix, rightSide, sol, Iterator, Preconditioner);

        return sol;
    }
}