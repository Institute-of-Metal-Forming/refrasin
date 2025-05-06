using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.Optimization.LineSearch;
using RefraSin.Numerics.Exceptions;
using RefraSin.Numerics.LinearSolvers;
using Serilog;

namespace RefraSin.Numerics.RootFinding;

public class NewtonRaphsonRootFinder(
    ILinearSolver jacobianStepSolver,
    bool useLineSearch = false,
    int maxIterationCount = 100,
    double absoluteTolerance = 1e-8,
    WolfeLineSearch? wolfeLineSearch = null
) : IRootFinder
{
    /// <inheritdoc />
    public Vector<double> FindRoot(
        Func<Vector<double>, Vector<double>> function,
        Func<Vector<double>, Matrix<double>> jacobian,
        Vector<double> initialGuess
    )
    {
        var logger = Log.ForContext<NewtonRaphsonRootFinder>();
        int i;

        var x = initialGuess.Clone();
        var y = function(x);

        var objectiveFunction = ObjectiveFunction.Gradient(
            v => function(v).L2Norm(),
            v => jacobian(v).ColumnNorms(2)
        );

        for (i = 1; i < MaxIterationCount; i++)
        {
            var jac = jacobian(x);
            jac.CoerceZero(1e-8);
            var dx = JacobianStepSolver.Solve(jac, -y);
            logger.Debug("Next step {Step}.", dx);

            if (!dx.ForAll(double.IsFinite))
                throw new UncriticalIterationInterceptedException(
                    nameof(NewtonRaphsonRootFinder),
                    InterceptReason.InvalidStateOccured,
                    i,
                    furtherInformation: "Infinite step occured."
                );

            if (UseLineSearch)
            {
                objectiveFunction.EvaluateAt(x);
                var lineSearchResult = WolfeLineSearch.FindConformingStep(
                    objectiveFunction,
                    dx,
                    0.5,
                    1
                );
                x = lineSearchResult.MinimizingPoint;
                logger.Debug("Line search resulted in step {Step}.", lineSearchResult.FinalStep);
            }
            else
            {
                x += dx;
            }

            y = function(x);
            var error = y.L2Norm();
            logger.Debug("Current error {Error}.", error);

            if (error <= AbsoluteTolerance)
            {
                logger.Debug("Solution found after {Iterations} iterations.", i);
                return x;
            }
        }

        logger.Warning("Maximum iteration count exceeded. Continuing anyway.");
        return x;
    }

    public int MaxIterationCount { get; } = maxIterationCount;

    public double AbsoluteTolerance { get; } = absoluteTolerance;

    public ILinearSolver JacobianStepSolver { get; } = jacobianStepSolver;

    public bool UseLineSearch { get; } = useLineSearch;

    public WolfeLineSearch WolfeLineSearch { get; } =
        wolfeLineSearch ?? new WeakWolfeLineSearch(1e-4, 0.9, 0.1, 100);
}
