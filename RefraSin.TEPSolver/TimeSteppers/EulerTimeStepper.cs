using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.TimeSteppers;

public class EulerTimeStepper : ITimeStepper
{
    /// <inheritdoc />
    public StepVector Step(ISolverSession solverSession, SolutionState baseState)
    {
        var equationSystem = solverSession.Routines.EquationSystemBuilder.Build(baseState);

        try
        {
            var step = solverSession.Routines.LagrangianRootFinder.FindRoot(
                equationSystem,
                solverSession.Routines.StepEstimator.EstimateStep(solverSession, equationSystem)
            );
            return step;
        }
        catch (Exception e)
        {
            throw new StepFailedException(innerException: e);
        }
    }

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}
