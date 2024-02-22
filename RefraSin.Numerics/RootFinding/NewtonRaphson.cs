using MathNet.Numerics.LinearAlgebra;
using RefraSin.Numerics.Exceptions;

namespace RefraSin.Numerics.RootFinding;

public class NewtonRaphsonRootFinder(
    int maxIterattionCount = 100,
    double absoluteTolerance = 1e-8,
    double averageDecreaseRateFactor = 1e-4,
    double minStepFactor = 0.1,
    double maxStepFactor = 0.5
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
            var dx = jac.LU().Solve(-y);

            if (!dx.ForAll(double.IsFinite))
                throw new UncriticalIterationInterceptedException(nameof(NewtonRaphsonRootFinder), InterceptReason.InvalidStateOccured, i,
                    furtherInformation: "Infinite step occured.");

            var gradf = -y * y / dx;

            var xnew = x + dx;
            var ynew = function(xnew);
            var fnew = 0.5 * ynew * ynew;

            if (fnew > f + AverageDecreaseRateFactor * gradf * (xnew - x))
            {
                var gprime0 = gradf * dx;

                var stepFactor = gprime0 / (2 * (fnew - f - gprime0));

                if (stepFactor > MaxStepFactor) stepFactor = MaxStepFactor;
                else if (stepFactor < MinStepFactor) stepFactor = MinStepFactor;

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

    public double MinStepFactor { get; } = minStepFactor;

    public double MaxStepFactor { get; } = maxStepFactor;
}