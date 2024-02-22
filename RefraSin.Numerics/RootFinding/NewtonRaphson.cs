using MathNet.Numerics.LinearAlgebra;
using RefraSin.Numerics.Exceptions;

namespace RefraSin.Numerics.RootFinding;

public class NewtonRaphsonRootFinder(
    int maxIterattionCount = 100,
    double absoluteTolerance = 1e-8,
    double averageDecreaseRateFactor = 1e-4,
    double minStepFraction = 0.1,
    double maxStepFraction = 0.5
) : IRootFinder
{
    /// <inheritdoc />
    public Vector<double> FindRoot(
        Func<Vector<double>, Vector<double>> function,
        Func<Vector<double>, Matrix<double>> jacobian,
        Vector<double> initialGuess
    )
    {
        int i;

        var x = initialGuess;
        var y = function(x);
            var f = 0.5 * y * y;

        for (i = 0; i < MaxIterationCount; i++)
        {
            if (y.L2Norm() <= AbsoluteTolerance)
            {
                return x;
            }

            var jac = jacobian(x);
            var dx = jac.LU().Solve(y * -1);
            var gradf = -f / dx;

            var xnew = x + dx;
            var ynew = function(xnew);
            var fnew = 0.5 * ynew * ynew;

            if (fnew > f + AverageDecreaseRateFactor * gradf * (xnew - x))
            {
                var gprime0 = gradf * dx;
                var g0 = f;
                var g1 = fnew;

                var stepFactor = gprime0 / (2 * (g1 - g0 - gprime0));

                if (stepFactor > MaxStepFraction) stepFactor = MaxStepFraction;
                else if (stepFactor < MinStepFraction) stepFactor = MinStepFraction;

                dx *= stepFactor;
                xnew = x + dx;
                ynew = function(xnew);
                fnew = 0.5 * ynew * ynew;
            }

            x = xnew;
            y = ynew;
            f = fnew;
        }

        throw new UncriticalIterationInterceptedException(nameof(NewtonRaphsonRootFinder), InterceptReason.MaxIterationCountExceeded, i);
    }

    public int MaxIterationCount { get; } = maxIterattionCount;

    public double AbsoluteTolerance { get; } = absoluteTolerance;

    public double AverageDecreaseRateFactor { get; } = averageDecreaseRateFactor;

    public double MinStepFraction { get; } = minStepFraction;

    public double MaxStepFraction { get; } = maxStepFraction;
}