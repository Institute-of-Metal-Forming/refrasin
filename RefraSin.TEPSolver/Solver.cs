using MathNet.Numerics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using RefraSin.TEPSolver.Exceptions;
using RefraSin.TEPSolver.Step;

namespace RefraSin.TEPSolver;

/// <summary>
/// Solver for performing time integration of sintering processes based on the thermodynamic extremal principle (TEP).
/// </summary>
public class Solver
{
    private SolverSession? _session;

    /// <summary>
    /// Numeric options to control solver behavior.
    /// </summary>
    public ISolverOptions Options { get; set; } = new SolverOptions();

    /// <summary>
    /// Storage for solution data.
    /// </summary>
    public ISolutionStorage SolutionStorage { get; set; } = new InMemorySolutionStorage();

    /// <summary>
    /// Factory for loggers used in the Session.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; } = new NullLoggerFactory();

    internal SolverSession Session
    {
        get => _session ?? throw new InvalidOperationException("Solution procedure is not initialized.");
        private set => _session = value;
    }

    /// <summary>
    /// Creates a new solver Session for the given process.
    /// </summary>
    internal SolverSession CreateSession(ISinteringProcess process)
    {
        var session = new SolverSession(this, process);
        Session = session;
        return session;
    }

    /// <summary>
    /// Run the solution procedure starting with the given state till the specified time.
    /// </summary>
    public void Solve(ISinteringProcess process)
    {
        CreateSession(process);
        DoTimeIntegration();
    }

    private void DoTimeIntegration()
    {
        Session.StoreCurrentState();

        while (Session.CurrentTime < Session.EndTime)
        {
            var stepVector = TrySolveStepUntilValid();
            Session.LastStep = stepVector;
            var particleTimeSteps = GenerateTimeStepsFromGradientSolution(stepVector).ToArray();
            Session.StoreStep(particleTimeSteps);

            Session.IncreaseCurrentTime();

            foreach (var timeStep in particleTimeSteps)
                Session.Particles[timeStep.ParticleId].ApplyTimeStep(stepVector, Session.TimeStepWidth);

            Session.StoreCurrentState();
            Session.MayIncreaseTimeStepWidth();
        }

        Session.Logger.LogInformation("End time successfully reached after {StepCount} steps.", Session.TimeStepIndex + 1);
    }

    internal StepVector TrySolveStepUntilValid()
    {
        int i;

        for (i = 0; i < Session.Options.MaxIterationCount; i++)
        {
            try
            {
                var step = TrySolveStepWithLastStepOrGuess();

                // step = MakeAdamsMoultonCorrection(step);

                CheckForInstability(step);

                return step;
            }
            catch (Exception e)
            {
                Session.Logger.LogError(e, "Exception occured during time step solution. Lowering time step width and try again.");
                Session.DecreaseTimeStepWidth();

                if (e is InstabilityException)
                {
                    Session.ResetTo(Session.StateMemory.Pop());
                }
            }
        }

        throw new CriticalIterationInterceptedException(nameof(TrySolveStepUntilValid), InterceptReason.MaxIterationCountExceeded, i);
    }

    private StepVector MakeAdamsMoultonCorrection(StepVector step)
    {
        if (Session.LastStep is not null)
            return (step + Session.LastStep) / 2;
        return step;
    }

    private void CheckForInstability(StepVector step)
    {
        foreach (var particle in Session.Particles.Values)
        {
            var displacements = particle.Nodes.Select(n => step[n].NormalDisplacement).ToArray();
            var differences = displacements.Zip(displacements.Skip(1).Append(displacements[0]), (current, next) => next - current).ToArray();

            for (int i = 0; i < differences.Length; i++)
            {
                if (
                    differences[i] * differences[(i + 1) % differences.Length] < 0 &&
                    differences[(i + 1) % differences.Length] * differences[(i + 2) % differences.Length] < 0 &&
                    differences[(i + 2) % differences.Length] * differences[(i + 3) % differences.Length] < 0
                )
                    throw new InstabilityException(particle.Id, particle.Nodes[i].Id, i);
            }
        }
    }

    private StepVector TrySolveStepWithLastStepOrGuess()
    {
        var lagrangianGradient = new LagrangianGradient(Session);

        try
        {
            return lagrangianGradient.Solve(Session.LastStep ?? lagrangianGradient.GuessSolution());
        }
        catch (NonConvergenceException e)
        {
            return lagrangianGradient.Solve(lagrangianGradient.GuessSolution());
        }
    }

    private IEnumerable<IParticleTimeStep> GenerateTimeStepsFromGradientSolution(StepVector stepVector)
    {
        foreach (var p in Session.Particles.Values)
            yield return new ParticleTimeStep(
                p.Id,
                stepVector[p].RadialDisplacement,
                stepVector[p].AngleDisplacement,
                stepVector[p].RotationDisplacement,
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