using MathNet.Numerics.RootFinding;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.Step;
using Node = RefraSin.TEPSolver.ParticleModel.Node;

namespace RefraSin.TEPSolver;

internal class LagrangianGradient
{
    private readonly ISolverSession _session;

    public LagrangianGradient(ISolverSession session)
    {
        _session = session;
    }

    public StepVector Solve(StepVector initialGuess)
    {
        double[] Fun(double[] vector) =>
            EvaluateLagrangianGradientAt(new StepVector(vector, initialGuess.StepVectorMap)).AsArray();

        return new StepVector(Broyden.FindRoot(
            Fun,
            initialGuess: initialGuess.AsArray(),
            maxIterations: _session.Options.RootFindingMaxIterationCount,
            accuracy: _session.Options.RootFindingAccuracy
        ), initialGuess.StepVectorMap);
    }

    private StepVector EvaluateLagrangianGradientAt(StepVector stepVector)
    {
        var evaluation = YieldEquations(stepVector).ToArray();

        if (evaluation.Any(x => !double.IsFinite(x)))
        {
            throw new InvalidOperationException("One ore more components of the gradient evaluated to an infinite value.");
        }

        return new StepVector(evaluation, stepVector.StepVectorMap);
    }

    private IEnumerable<double> YieldEquations(StepVector stepVector)
    {
        // fix root particle to origin
        var root = _session.Particles.Values.First()!;
        yield return stepVector[root].RadialDisplacement;
        yield return stepVector[root].AngleDisplacement;
        yield return stepVector[root].RotationDisplacement;

        // yield particle displacement equations
        foreach (var particle in _session.Particles.Values.Skip(1))
        {
            yield return stepVector[particle].RadialDisplacement;
            yield return stepVector[particle].AngleDisplacement;
            yield return stepVector[particle].RotationDisplacement;
        }

        // yield node equations
        foreach (var node in _session.Nodes.Values)
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
            2 * _session.GasConstant * _session.Temperature
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
        var dissipation = _session.Nodes.Values.Select(n =>
            -n.GibbsEnergyGradient.Normal * stepVector[n].NormalDisplacement
        ).Sum();

        var dissipationFunction = _session.GasConstant * _session.Temperature / 2
                                * _session.Nodes.Values.Select(n =>
                                      (
                                          n.SurfaceDistance.ToUpper * Math.Pow(stepVector[n].FluxToUpper, 2) / n.SurfaceDiffusionCoefficient.ToUpper
                                        + n.SurfaceDistance.ToLower * Math.Pow(stepVector[n.Lower].FluxToUpper, 2) /
                                          n.SurfaceDiffusionCoefficient.ToLower
                                      ) / (n.Particle.Material.MolarVolume * n.Particle.Material.EquilibriumVacancyConcentration)
                                  ).Sum();

        return dissipation - dissipationFunction;
    }

    public StepVector GuessSolution() => new(YieldInitialGuess().ToArray(), new StepVectorMap(_session.Particles.Values, _session.Nodes.Values));

    private IEnumerable<double> YieldInitialGuess() =>
        YieldGlobalUnknownsInitialGuess()
            .Concat(YieldParticleUnknownsInitialGuess()
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
        foreach (var particle in _session.Particles.Values)
        {
            yield return 0;
            yield return 0;
            yield return 0;
        }
    }

    private IEnumerable<double> YieldNodeUnknownsInitialGuess()
    {
        foreach (var node in _session.Nodes.Values)
        {
            yield return node.GuessNormalDisplacement();
            yield return node.GuessFluxToUpper();
            yield return 1;
        }
    }
}