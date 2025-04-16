using MathNet.Numerics.LinearAlgebra;
using RefraSin.Numerics.RootFinding;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public class DirectLagrangianRootFinder(IRootFinder rootFinder) : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(EquationSystem equationSystem, StepVector initialGuess)
    {
        var solution = RootFinder.FindRoot(Fun, Jac, initialGuess);

        return new StepVector(solution, initialGuess.StepVectorMap);

        Vector<double> Fun(Vector<double> vector)
        {
            var result = equationSystem.Lagrangian(
                new StepVector(vector, initialGuess.StepVectorMap)
            );
            return result;
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            var result = equationSystem.Jacobian(
                new StepVector(vector, initialGuess.StepVectorMap)
            );
            return result;
        }
    }

    public IRootFinder RootFinder { get; } = rootFinder;

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
