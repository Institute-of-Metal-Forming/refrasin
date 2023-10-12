using MathNet.Numerics;
using MathNet.Numerics.RootFinding;
using RefraSin.TEPSolver.Step;
using static System.Math;

namespace RefraSin.TEPSolver;

internal class LagrangianGradient
{
    public LagrangianGradient(ISolverSession solverSession)
    {
        SolverSession = solverSession;
        StepVectorMap = new StepVectorMap(solverSession);

        Solution = GuessSolution();
    }

    public ISolverSession SolverSession { get; }
    public StepVectorMap StepVectorMap { get; }

    public StepVector Solution { get; private set; }

    public StepVector EvaluateAt(StepVector state)
    {
        var evaluation = YieldEquations(state).ToArray();

        if (evaluation.Any(x => !double.IsFinite(x)))
        {
            throw new InvalidOperationException("One ore more components of the gradient evaluated to an infinite value.");
        }

        return new StepVector(evaluation, StepVectorMap);
    }

    private double[] EvaluateAtArray(double[] state) => EvaluateAt(new StepVector(state, StepVectorMap)).AsArray();

    private IEnumerable<double> YieldEquations(StepVector state) =>
        YieldStateVelocityDerivatives(state)
            .Concat(
                YieldFluxDerivatives(state)
            )
            .Concat(
                YieldDissipationEquality(state)
            )
            .Concat(
                YieldRequiredConstraints(state)
            );

    private IEnumerable<double> YieldStateVelocityDerivatives(StepVector state)
    {
        foreach (var node in SolverSession.Nodes.Values)
        {
            // Normal Displacement
            var gibbsTerm = -node.GibbsEnergyGradient.Normal * (1 + state.Lambda1);
            var requiredConstraintsTerm = node.VolumeGradient.Normal * state[node].Lambda2;

            yield return gibbsTerm + requiredConstraintsTerm;
        }
    }

    private IEnumerable<double> YieldFluxDerivatives(StepVector state)
    {
        foreach (var node in SolverSession.Nodes.Values) // for each flux
        {
            // Flux To Upper
            var dissipationTerm =
                2 * SolverSession.GasConstant * SolverSession.Temperature * SolverSession.TimeStepWidth
              / (node.Particle.Material.MolarVolume * node.Particle.Material.EquilibriumVacancyConcentration)
              * node.SurfaceDistance.ToUpper * state[node].FluxToUpper / node.SurfaceDiffusionCoefficient.ToUpper
              * state.Lambda1;
            var thisRequiredConstraintsTerm = SolverSession.TimeStepWidth * state[node].Lambda2;
            var upperRequiredConstraintsTerm = SolverSession.TimeStepWidth * state[node.Upper].Lambda2;

            yield return -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
        }
    }

    private IEnumerable<double> YieldDissipationEquality(StepVector state)
    {
        var dissipation = SolverSession.Nodes.Values.Select(n =>
            -n.GibbsEnergyGradient.Normal * state[n].NormalDisplacement
        ).Sum();

        var dissipationFunction =
            SolverSession.GasConstant * SolverSession.Temperature * SolverSession.TimeStepWidth / 2
          * SolverSession.Nodes.Values.Select(n =>
                (
                    n.SurfaceDistance.ToUpper * Pow(state[n].FluxToUpper, 2) / n.SurfaceDiffusionCoefficient.ToUpper
                  + n.SurfaceDistance.ToLower * Pow(state[n.Lower].FluxToUpper, 2) / n.SurfaceDiffusionCoefficient.ToLower
                ) / (n.Particle.Material.MolarVolume * n.Particle.Material.EquilibriumVacancyConcentration)
            ).Sum();

        yield return dissipation - dissipationFunction;
    }

    private IEnumerable<double> YieldRequiredConstraints(StepVector state)
    {
        foreach (var node in SolverSession.Nodes.Values)
        {
            var volumeTerm = node.VolumeGradient.Normal * state[node].NormalDisplacement;
            var fluxTerm =
                SolverSession.TimeStepWidth *
                (
                    state[node].FluxToUpper
                  - state[node.Lower].FluxToUpper
                );

            yield return volumeTerm - fluxTerm;
        }
    }

    public StepVector FindRoot()
    {
        StepVector solution;

        try
        {
            solution = new StepVector(Broyden.FindRoot(
                EvaluateAtArray,
                initialGuess: Solution.AsArray(),
                maxIterations: SolverSession.Options.RootFindingMaxIterationCount,
                accuracy: SolverSession.Options.RootFindingAccuracy
            ), StepVectorMap);
        }
        catch (NonConvergenceException e)
        {
            solution = new StepVector(Broyden.FindRoot(
                EvaluateAtArray,
                initialGuess: GuessSolution().AsArray(),
                maxIterations: SolverSession.Options.RootFindingMaxIterationCount,
                accuracy: SolverSession.Options.RootFindingAccuracy
            ), StepVectorMap);
        }

        Solution = solution;
        return solution;
    }

    public StepVector GuessSolution() => new(YieldInitialGuess().ToArray(), StepVectorMap);

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
        yield break;

        foreach (var particle in SolverSession.Particles.Values) { }
    }

    private IEnumerable<double> YieldNodeUnknownsInitialGuess()
    {
        foreach (var node in SolverSession.Nodes.Values)
        {
            yield return node.GuessNormalDisplacement();
            yield return node.GuessFluxToUpper();
            ;
            yield return 1;
        }
    }
}