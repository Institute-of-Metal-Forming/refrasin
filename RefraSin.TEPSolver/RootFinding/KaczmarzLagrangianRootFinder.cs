using Microsoft.Extensions.Logging;
using RefraSin.Numerics.Exceptions;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

namespace RefraSin.TEPSolver.RootFinding;


/// <summary>
/// Kaczmarz solver inspired by L. Föcke - Inverse Problems for Random Measurements
/// </summary>
public class KaczmarzLagrangianRootFinder(
    int randomSeed = 980323704,
    int maximumEpochCount = 1000,
    int epochSize = 1,
    double precision = 1e-8,
    double stepBreakFactor = 1
) : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(SolutionState currentState, StepVector initialGuess, ILogger logger)
    {
        int epoch;
        var stepVector = initialGuess.Copy();
        var system = new EquationSystem.EquationSystem(currentState, stepVector);
        var equationCount = system.Equations.Count;
        var epochSize = equationCount * EpocSize;
        var randomSource = new MersenneTwister(RandomSeed);

        for (epoch = 0; epoch < MaximumEpochCount; epoch++)
        {
            var systemResidual = system.Lagrangian();
            var systemResidualNorm = systemResidual.L2Norm();

            if (systemResidualNorm < Precision)
                return stepVector;

            var distribution = new Categorical(systemResidual.Select(r => Abs(r) / systemResidualNorm).ToArray(), randomSource);

            int randomEquation = distribution.Sample();

            for (var use = 0; use < epochSize; use++)
            {
                var equation = system.Equations[randomEquation];
                var equationResidual = equation.Value();
                var equationDerivatives = equation.Derivative().ToArray();
                var stepNorm = equationDerivatives.Sum(d => Pow(d.value, 2));

                // var i = randomSource.Next(equationDerivatives.Length);
                // var derivative = equationDerivatives[i].value;
                
                foreach (var (i, derivative) in equationDerivatives)
                {
                    if (double.IsFinite(derivative) && Abs(derivative) > 1e-2)
                    {
                        var step = StepBreakFactor / stepNorm * equationResidual / derivative;
                        stepVector[i] -= step;
                    }
                }

                randomEquation = distribution.Sample();
            }
        }

        throw new UncriticalIterationInterceptedException(
            nameof(KaczmarzLagrangianRootFinder),
            InterceptReason.MaxIterationCountExceeded,
            epoch
        );
    }

    public int MaximumEpochCount { get; } = maximumEpochCount;

    public int EpocSize { get; } = epochSize;

    public int RandomSeed { get; } = randomSeed;

    public double Precision { get; } = precision;

    public double StepBreakFactor { get; } = stepBreakFactor;

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
