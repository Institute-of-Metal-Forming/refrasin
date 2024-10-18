using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.RootFinding;
using Microsoft.Extensions.Logging;
using RefraSin.Numerics.Exceptions;
using RefraSin.Numerics.RootFinding;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public class DirectLagrangianRootFinder(
    IRootFinder rootFinder,
    double iterationPrecision = 1e-4,
    int maxIterationCount = 100
) : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(SolutionState currentState, StepVector initialGuess, ILogger logger)
    {
        var stepVector = initialGuess.Copy();
        var system = new EquationSystem.EquationSystem(currentState, stepVector);

        var solution = RootFinder.FindRoot(
            Fun,
            Jac,
            system.StepVector,
            logger
        );
        
        system.StepVector.Update(solution.AsArray());

        return stepVector;

        Vector<double> Fun(Vector<double> vector)
        {
            system.StepVector.Update(vector.AsArray()[..^1]);
            var result = system.Lagrangian();
            return result;
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            var result = system.Jacobian();
            return result;
        }
    }

    public IRootFinder RootFinder { get; } = rootFinder;

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
