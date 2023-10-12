using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.RootFinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MoreLinq;
using RefraSin.Coordinates.Polar;
using RefraSin.Iteration;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver;

/// <summary>
/// Solver for performing time integration of sintering processes based on the thermodynamic extremal principle (TEP).
/// </summary>
public partial class Solver
{
    /// <summary>
    /// Numeric options to control solver behavior.
    /// </summary>
    public ISolverOptions Options { get; set; } = new SolverOptions();

    /// <summary>
    /// Storage for solution data.
    /// </summary>
    public ISolutionStorage SolutionStorage { get; set; } = new InMemorySolutionStorage();

    /// <summary>
    /// Factory for loggers used in the session.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; } = new NullLoggerFactory();

    /// <summary>
    /// Creates a new solver session for the given process.
    /// </summary>
    internal SolverSession CreateSession(ISinteringProcess process) => new SolverSession(this, process);

    /// <summary>
    /// Run the solution procedure starting with the given state till the specified time.
    /// </summary>
    public void Solve(ISinteringProcess process)
    {
        var session = CreateSession(process);
        DoTimeIntegration(session);
    }

    private static void DoTimeIntegration(ISolverSession session)
    {
        session.StoreCurrentState();

        while (session.CurrentTime < session.EndTime)
        {
            var stepVector = TrySolveStepUntilValid(session);
            var particleTimeSteps = GenerateTimeStepsFromGradientSolution(session, stepVector).ToArray();
            session.StoreStep(particleTimeSteps);

            session.IncreaseCurrentTime();

            foreach (var timeStep in particleTimeSteps)
                session.Particles[timeStep.ParticleId].ApplyTimeStep(timeStep);

            session.StoreCurrentState();
            session.MayIncreaseTimeStepWidth();
        }

        session.Logger.LogInformation("End time successfully reached after {StepCount} steps.", session.TimeStepIndex + 1);
    }

    private static StepVector TrySolveStepUntilValid(ISolverSession session)
    {
        int i;

        for (i = 0; i < session.Options.MaxIterationCount; i++)
        {
            try
            {
                var particleTimeSteps = SolveStep(session);

                return particleTimeSteps;
            }
            catch (Exception e)
            {
                session.Logger.LogError(e, "Exception occured during time step solution. Lowering time step width and try again.");
                session.DecreaseTimeStepWidth();
            }
        }

        throw new CriticalIterationInterceptedException(nameof(TrySolveStepUntilValid), InterceptReason.MaxIterationCountExceeded, i);
    }

    private static StepVector SolveStep(ISolverSession session) => session.LagrangianGradient.FindRoot();

    private static IEnumerable<IParticleTimeStep> GenerateTimeStepsFromGradientSolution(ISolverSession session, StepVector stepVector)
    {
        foreach (var p in session.Particles.Values)
            yield return new ParticleTimeStep(
                p.Id,
                0,
                0,
                0,
                p.Surface.Select(n =>
                    new NodeTimeStep(n.Id,
                        stepVector[n].NormalDisplacement,
                        0,
                        new ToUpperToLower(
                            stepVector[n].FluxToUpper,
                            -stepVector[n].FluxToUpper
                        ),
                        0
                    )));
    }
}