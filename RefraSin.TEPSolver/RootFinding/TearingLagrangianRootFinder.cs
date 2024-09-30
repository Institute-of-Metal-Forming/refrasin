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

public class TearingLagrangianRootFinder(
    IRootFinder particleBlockRootFinder,
    IRootFinder borderBlockRootFinder,
    double iterationPrecision = 1e-4,
    int maxIterationCount = 100
) : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(SolutionState currentState, StepVector initialGuess, ILogger logger)
    {
        var stepVector = initialGuess.Copy();

        var dissipationEqualitySolution = SolveDissipationEquality(
            currentState,
            stepVector,
            logger
        );
        stepVector[^1] = dissipationEqualitySolution;
        return stepVector;
    }

    private double SolveDissipationEquality(
        SolutionState currentState,
        StepVector stepVector,
        ILogger logger
    )
    {
        var solution = Brent.FindRootExpand(
            Fun,
            stepVector.LambdaDissipation() * 0.9,
            stepVector.LambdaDissipation() * 1.1
        );

        return solution;

        double Fun(double lambdaDissipation)
        {
            stepVector[^1] = lambdaDissipation;

            stepVector = SolveParticleAndBorderBlocks(currentState, stepVector, logger);

            var result = Lagrangian.DissipationEquality(currentState, stepVector);
            return result;
        }
    }

    private StepVector SolveParticleAndBorderBlocks(
        SolutionState currentState,
        StepVector stepVector,
        ILogger logger
    )
    {
        int i;

        for (i = 0; i < MaxIterationCount; i++)
        {
            var oldVector = stepVector.Copy();

            foreach (var particle in currentState.Particles)
            {
                var particleSolution = SolveParticleBlock(particle, stepVector, logger);
                stepVector.UpdateParticleBlock(particle, particleSolution.AsArray());
            }

            var borderSolution = SolveBorderBlockWithoutDissipationEquality(
                currentState,
                stepVector,
                logger
            );
            stepVector.UpdateBorderBlock(borderSolution.AsArray());

            if ((stepVector - oldVector).L2Norm() < IterationPrecision * stepVector.L2Norm())
            {
                logger.LogInformation("{Loop} iterations: {Count}", nameof(SolveParticleAndBorderBlocks), i);
                return stepVector;
            }
        }

        throw new UncriticalIterationInterceptedException(
            $"{nameof(TearingLagrangianRootFinder)}.{nameof(FindRoot)}",
            InterceptReason.MaxIterationCountExceeded,
            i
        );
    }

    private Vector<double> SolveBorderBlockWithoutDissipationEquality(
        SolutionState currentState,
        StepVector stepVector,
        ILogger logger
    )
    {
        var solution = BorderBlockRootFinder.FindRoot(
            Fun,
            Jac,
            new DenseVector(stepVector.BorderBlock()[..^1]),
            logger
        );

        return solution;

        Vector<double> Fun(Vector<double> vector)
        {
            stepVector.UpdateBorderBlock(vector.AsArray());
            var result = Lagrangian
                .YieldLinearBorderBlockEquations(currentState, stepVector)
                .ToArray();
            return new DenseVector(result);
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            stepVector.UpdateBorderBlock(vector.AsArray());
            return Jacobian.LinearBorderBlock(currentState, stepVector);
        }
    }

    private Vector<double> SolveParticleBlock(
        Particle particle,
        StepVector stepVector,
        ILogger logger
    )
    {
        var solution = ParticleBlockRootFinder.FindRoot(
            Fun,
            Jac,
            new DenseVector(stepVector.ParticleBlock(particle)),
            logger
        );

        return solution;

        Vector<double> Fun(Vector<double> vector)
        {
            stepVector.UpdateParticleBlock(particle, vector.AsArray());
            var result = Lagrangian.YieldParticleBlockEquations(particle, stepVector).ToArray();
            return new DenseVector(result);
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            stepVector.UpdateParticleBlock(particle, vector.AsArray());
            var result = Jacobian.ParticleBlock(particle, stepVector);
            return result;
        }
    }

    public IRootFinder ParticleBlockRootFinder { get; } = particleBlockRootFinder;

    public IRootFinder BorderBlockRootFinder { get; } = borderBlockRootFinder;
    public int MaxIterationCount { get; } = maxIterationCount;

    public double IterationPrecision { get; } = iterationPrecision;

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
