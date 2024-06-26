using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using RefraSin.Numerics.Exceptions;
using RefraSin.Numerics.RootFinding;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public class TearingLagrangianRootFinder(IRootFinder particleBlockRootFinder, IRootFinder borderBlockRootFinder) : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(
        ISolverSession solverSession,
        SolutionState currentState,
        StepVector initialGuess
    )
    {
        var stepVector = initialGuess.Copy();

        int i;

        for (i = 0; i < solverSession.Options.MaxIterationCount; i++)
        {
            var oldVector = stepVector.Copy();

            foreach (var particle in currentState.Particles)
            {
                var particleSolution = SolveParticleBlock(particle, stepVector);
                stepVector.UpdateParticleBlock(particle, particleSolution.AsArray());
            }

            var borderSolution = SolveBorderBlock(currentState, stepVector);
            stepVector.UpdateBorderBlock(borderSolution.AsArray());

            if ((stepVector - oldVector).L2Norm() < solverSession.Options.IterationPrecision * stepVector.L2Norm())
                return stepVector;
        }

        throw new UncriticalIterationInterceptedException($"{nameof(TearingLagrangianRootFinder)}.{nameof(FindRoot)}",
            InterceptReason.MaxIterationCountExceeded, i);
    }

    private Vector<double> SolveBorderBlock(
        SolutionState currentState,
        StepVector stepVector
    )
    {
        var solution = BorderBlockRootFinder.FindRoot(
            Fun,
            Jac,
            new DenseVector(stepVector.BorderBlock())
        );

        return solution;

        Vector<double> Fun(Vector<double> vector)
        {
            stepVector.UpdateBorderBlock(vector.AsArray());
            var result = Lagrangian
                .YieldBorderBlockEquations(currentState, stepVector)
                .ToArray();
            return new DenseVector(result);
        }

        Matrix<double> Jac(Vector<double> vector)
        {
            stepVector.UpdateBorderBlock(vector.AsArray());
            var result = Jacobian.BorderBlock(currentState, stepVector);
            return result;
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
}