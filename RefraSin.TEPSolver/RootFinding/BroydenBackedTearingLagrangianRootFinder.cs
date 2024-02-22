using MathNet.Numerics.RootFinding;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public class BroydenBackedTearingLagrangianRootFinder : ILagrangianRootFinder
{
    /// <inheritdoc />
    public StepVector FindRoot(ISolverSession solverSession, SolutionState currentState, StepVector initialGuess)
    {
        var stepVector = initialGuess.Copy();

        // Broyden Algorithm copied from MathNet.Numerics.LinearAlgebra.Broyden and modified with exact Jacobian

        double[] Fun(double[] vector)
        {
            stepVector.UpdateBorderBlock(vector);
            var result = TearAndEvaluateFunctionalBlock(solverSession, currentState, stepVector);
            return result;
        }

        var solution = Broyden.FindRoot(
            Fun,
            initialGuess: stepVector.BorderBlock(),
            accuracy: solverSession.Options.RootFindingAccuracy,
            maxIterations: solverSession.Options.RootFindingMaxIterationCount
        );

        stepVector.UpdateBorderBlock(solution);
        return stepVector;
    }

    private static double[] TearAndEvaluateFunctionalBlock(ISolverSession solverSession, SolutionState currentState,
        StepVector stepVector)
    {
        foreach (var particle in currentState.Particles)
        {
            var particleSolution = SolveParticleBlock(solverSession, particle, stepVector);
            stepVector.UpdateParticleBlock(particle, particleSolution);
        }

        return Lagrangian.YieldFunctionalBlockEquations(solverSession, currentState, stepVector).ToArray();
    }

    private static double[] SolveParticleBlock(ISolverSession solverSession, Particle particle, StepVector stepVector)
    {
        double[] Fun(double[] vector)
        {
            stepVector.UpdateParticleBlock(particle, vector);
            var result = Lagrangian.YieldParticleBlockEquations(solverSession, particle, stepVector).ToArray();
            return result;
        }

        var solution = Broyden.FindRoot(
            Fun,
            initialGuess: stepVector.ParticleBlock(particle),
            accuracy: solverSession.Options.RootFindingAccuracy,
            maxIterations: solverSession.Options.RootFindingMaxIterationCount
        );

        return solution;
    }
}