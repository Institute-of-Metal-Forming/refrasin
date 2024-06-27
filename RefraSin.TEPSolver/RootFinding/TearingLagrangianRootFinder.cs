using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.RootFinding;
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
    int maxIterationCount = 100) : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(
        ISolverSession solverSession,
        SolutionState currentState,
        StepVector initialGuess
    )
    {
        var stepVector = initialGuess.Copy();

        var dissipationEqualitySolution = SolveDissipationEquality(currentState, stepVector);
        stepVector[^1] = dissipationEqualitySolution;
        return stepVector;
    }

    private double SolveDissipationEquality(SolutionState currentState, StepVector stepVector)
    {
        // var samples = Generate.LinearRangeMap(1e-4, 1e-4, 1e-2, Fun);
        var solution = Brent.FindRootExpand(Fun, stepVector.LambdaDissipation() * 0.9, stepVector.LambdaDissipation() * 1.1);

        return solution;

        double Fun(double lambdaDissipation)
        {
            stepVector[^1] = lambdaDissipation;

            stepVector = SolveParticleAndBorderBlocks(currentState, stepVector);

            var result = Lagrangian.DissipationEquality(currentState, stepVector);
            return result;
        }
    }

    private StepVector SolveParticleAndBorderBlocks(
        SolutionState currentState,
        StepVector stepVector
    )
    {
        int i;

        for (i = 0; i < MaxIterationCount; i++)
        {
            var oldVector = stepVector.Copy();

            foreach (var particle in currentState.Particles)
            {
                var particleSolution = SolveParticleBlock(particle, stepVector);
                stepVector.UpdateParticleBlock(particle, particleSolution.AsArray());
            }

            var borderSolution = SolveBorderBlockWithoutDissipationEquality(currentState, stepVector);
            stepVector.UpdateBorderBlock(borderSolution.AsArray());

            if ((stepVector - oldVector).L2Norm() < IterationPrecision * stepVector.L2Norm())
                return stepVector;
        }

        throw new UncriticalIterationInterceptedException($"{nameof(TearingLagrangianRootFinder)}.{nameof(FindRoot)}",
            InterceptReason.MaxIterationCountExceeded, i);
    }

    private Vector<double> SolveBorderBlockWithoutDissipationEquality(
        SolutionState currentState,
        StepVector stepVector
    )
    {
        var solution = BorderBlockRootFinder.FindRoot(
            Fun,
            Jac,
            new DenseVector(stepVector.BorderBlock()[..^1])
        );

        return solution;

        Vector<double> Fun(Vector<double> vector)
        {
            stepVector.UpdateBorderBlock(vector.AsArray());
            var result = Lagrangian
                .YieldBorderBlockEquations(currentState, stepVector)
                .SkipLast(1)
                .ToArray();
            return new DenseVector(result);
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            stepVector.UpdateBorderBlock(vector.AsArray());
            var result = Jacobian.BorderBlock(currentState, stepVector);
            return result.SubMatrix(0, result.RowCount - 1, 0, result.ColumnCount - 1);
        }
    }

    private Vector<double> SolveParticleBlock(
        Particle particle,
        StepVector stepVector
    )
    {
        var solution = ParticleBlockRootFinder.FindRoot(
            Fun,
            Jac,
            new DenseVector(stepVector.ParticleBlock(particle))
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
}