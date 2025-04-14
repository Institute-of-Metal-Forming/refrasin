using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.RootFinding;
using Microsoft.Extensions.Logging;
using RefraSin.Numerics.Exceptions;
using RefraSin.Numerics.RootFinding;
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
    public StepVector FindRoot(
        EquationSystem equationSystem,
        StepVector initialGuess,
        ILogger logger
    )
    {
        var solution = RootFinder.FindRoot(Fun, Jac, initialGuess, logger);

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
