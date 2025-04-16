using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using RefraSin.Numerics.Exceptions;
using RefraSin.Numerics.RootFinding;
using RefraSin.TEPSolver.StepVectors;
using Log = Serilog.Log;

namespace RefraSin.TEPSolver.RootFinding;

public class DirectLagrangianRootFinder(
    IRootFinder rootFinder,
    double minDissipation = 1e-2,
    int maxIterations = 10
) : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(EquationSystem equationSystem, StepVector initialGuess)
    {
        var map = initialGuess.StepVectorMap;

        for (int i = 0; i < MaxIterations; i++)
        {
            var solution = RootFinder.FindRoot(Fun, Jac, initialGuess);
            var step = new StepVector(solution, map);
            var dissipation = equationSystem.Dissipation(step);
            Log.ForContext<DirectLagrangianRootFinder>()
                .Debug("Root found with dissipation {Dissipation}", dissipation);

            if (dissipation > MinDissipation)
                return step;

            Log.Information(
                "Root finding converged to trivial solution. Trying again with larger initial guess."
            );
            initialGuess *= 2;
        }

        throw new NumericException("Failed to converge to non-trivial stationary point.");

        Vector<double> Fun(Vector<double> vector)
        {
            var result = equationSystem.Lagrangian(new StepVector(vector, map));
            return result;
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            var result = equationSystem.Jacobian(new StepVector(vector, map));
            return result;
        }
    }

    public IRootFinder RootFinder { get; } = rootFinder;

    public double MinDissipation { get; } = minDissipation;

    public int MaxIterations { get; } = maxIterations;

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
