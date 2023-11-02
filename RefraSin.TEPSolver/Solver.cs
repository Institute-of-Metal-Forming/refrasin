using MathNet.Numerics;
using MathNet.Numerics.RootFinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.Storage;
using RefraSin.TEPSolver.Exceptions;
using RefraSin.TEPSolver.Step;
using static System.Math;
using Node = RefraSin.TEPSolver.ParticleModel.Node;

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

    internal void DoTimeIntegration()
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

    // private StepVector GetRungeKuttaK(ISolutionState state, )

    private StepVector TrySolveStepWithLastStepOrGuess()
    {
        try
        {
            return SolveStep(Session.LastStep ?? GuessSolution());
        }
        catch (NonConvergenceException e)
        {
            return SolveStep(GuessSolution());
        }
    }

    private StepVector SolveStep(StepVector initialGuess) =>
        new(Broyden.FindRoot(
            EvaluateLagrangianGradientAt,
            initialGuess: initialGuess.AsArray(),
            maxIterations: Session.Options.RootFindingMaxIterationCount,
            accuracy: Session.Options.RootFindingAccuracy
        ), Session.StepVectorMap);

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

    private StepVector EvaluateLagrangianGradientAt(StepVector stepVector)
    {
        var evaluation = YieldEquations(stepVector).ToArray();

        if (evaluation.Any(x => !double.IsFinite(x)))
        {
            throw new InvalidOperationException("One ore more components of the gradient evaluated to an infinite value.");
        }

        return new StepVector(evaluation, Session.StepVectorMap);
    }

    private double[] EvaluateLagrangianGradientAt(double[] vector) =>
        EvaluateLagrangianGradientAt(new StepVector(vector, Session.StepVectorMap)).AsArray();

    private IEnumerable<double> YieldEquations(StepVector stepVector)
    {
        // fix root particle to origin
        var root = Session.Particles.Values.First()!;
        yield return stepVector[root].RadialDisplacement;
        yield return stepVector[root].AngleDisplacement;
        yield return stepVector[root].RotationDisplacement;

        // yield particle displacement equations
        foreach (var particle in Session.Particles.Values.Skip(1))
        {
            yield return stepVector[particle].RadialDisplacement;
            yield return stepVector[particle].AngleDisplacement;
            yield return stepVector[particle].RotationDisplacement;
        }

        // yield node equations
        foreach (var node in Session.Nodes.Values)
        {
            yield return StateVelocityDerivative(stepVector, node);
            yield return FluxDerivative(stepVector, node);
            yield return RequiredConstraint(stepVector, node);
        }

        yield return DissipationEquality(stepVector);
    }

    private double StateVelocityDerivative(StepVector stepVector, Node node)
    {
        var gibbsTerm = -node.GibbsEnergyGradient.Normal * (1 + stepVector.Lambda1);
        var requiredConstraintsTerm = node.VolumeGradient.Normal * stepVector[node].Lambda2;

        return gibbsTerm + requiredConstraintsTerm;
    }

    private double FluxDerivative(StepVector stepVector, Node node)
    {
        var dissipationTerm =
            2 * Session.GasConstant * Session.Temperature
          / (node.Particle.Material.MolarVolume * node.Particle.Material.EquilibriumVacancyConcentration)
          * node.SurfaceDistance.ToUpper * stepVector[node].FluxToUpper / node.SurfaceDiffusionCoefficient.ToUpper
          * stepVector.Lambda1;
        var thisRequiredConstraintsTerm = stepVector[node].Lambda2;
        var upperRequiredConstraintsTerm = stepVector[node.Upper].Lambda2;

        return -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
    }

    private double RequiredConstraint(StepVector stepVector, Node node)
    {
        var volumeTerm = node.VolumeGradient.Normal * stepVector[node].NormalDisplacement;
        var fluxTerm = stepVector[node].FluxToUpper - stepVector[node.Lower].FluxToUpper;

        return volumeTerm - fluxTerm;
    }

    private double DissipationEquality(StepVector stepVector)
    {
        var dissipation = Session.Nodes.Values.Select(n =>
            -n.GibbsEnergyGradient.Normal * stepVector[n].NormalDisplacement
        ).Sum();

        var dissipationFunction =
            Session.GasConstant * Session.Temperature / 2
          * Session.Nodes.Values.Select(n =>
                (
                    n.SurfaceDistance.ToUpper * Pow(stepVector[n].FluxToUpper, 2) / n.SurfaceDiffusionCoefficient.ToUpper
                  + n.SurfaceDistance.ToLower * Pow(stepVector[n.Lower].FluxToUpper, 2) / n.SurfaceDiffusionCoefficient.ToLower
                ) / (n.Particle.Material.MolarVolume * n.Particle.Material.EquilibriumVacancyConcentration)
            ).Sum();

        return dissipation - dissipationFunction;
    }

    private StepVector GuessSolution() => new(YieldInitialGuess().ToArray(), Session.StepVectorMap);

    private IEnumerable<double> YieldInitialGuess() =>
        YieldGlobalUnknownsInitialGuess()
            .Concat(
                YieldParticleUnknownsInitialGuess()
            )
            .Concat(
                YieldNodeUnknownsInitialGuess()
            );

    private IEnumerable<double> YieldGlobalUnknownsInitialGuess()
    {
        yield return 1;
    }

    private IEnumerable<double> YieldParticleUnknownsInitialGuess()
    {
        foreach (var particle in Session.Particles.Values)
        {
            yield return 0;
            yield return 0;
            yield return 0;
        }
    }

    private IEnumerable<double> YieldNodeUnknownsInitialGuess()
    {
        foreach (var node in Session.Nodes.Values)
        {
            yield return node.GuessNormalDisplacement();
            yield return node.GuessFluxToUpper();
            yield return 1;
        }
    }
}