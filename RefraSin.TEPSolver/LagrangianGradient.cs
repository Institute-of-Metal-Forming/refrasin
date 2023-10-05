using MathNet.Numerics;
using MathNet.Numerics.RootFinding;
using MoreLinq;
using RefraSin.TEPSolver.ParticleModel;
using static System.Math;

namespace RefraSin.TEPSolver;

internal class LagrangianGradient
{
    public LagrangianGradient(ISolverSession solverSession)
    {
        SolverSession = solverSession;

        GlobalUnknownsCount = Enum.GetNames(typeof(GlobalUnknown)).Length;

        ParticleUnknownsCount = Enum.GetNames(typeof(ParticleUnknown)).Length;
        ParticleCount = solverSession.Particles.Count;
        ParticleStartIndex = GlobalUnknownsCount;
        ParticleIndices = solverSession.Particles.Keys.Index().ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        NodeUnknownsCount = Enum.GetNames(typeof(NodeUnknown)).Length;
        NodeCount = solverSession.Nodes.Count;
        NodeStartIndex = ParticleStartIndex + ParticleCount * ParticleUnknownsCount;
        NodeIndices = solverSession.Nodes.Keys.Index().ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        TotalUnknownsCount = NodeStartIndex + NodeCount * NodeUnknownsCount;

        _solution = YieldInitialGuess().ToArray();
    }

    public ISolverSession SolverSession { get; }

    public int ParticleCount { get; }

    public int ParticleStartIndex { get; }

    public IReadOnlyDictionary<Guid, int> ParticleIndices { get; }

    public int NodeCount { get; }

    public int NodeStartIndex { get; }

    public IReadOnlyDictionary<Guid, int> NodeIndices { get; }

    public int TotalUnknownsCount { get; }

    private double[] _solution;

    public enum GlobalUnknown
    {
        Lambda1
    }

    public int GlobalUnknownsCount { get; }

    public enum ParticleUnknown
    {
        // RadialDisplacement,
        // AngleDisplacement,
        // RotationDisplacement
    }

    public int ParticleUnknownsCount { get; }

    public enum NodeUnknown
    {
        NormalDisplacement,
        FluxToUpper,
        Lambda2
    }

    public int NodeUnknownsCount { get; }

    public int GetIndex(GlobalUnknown unknown) => (int)unknown;

    public int GetIndex(Guid particleId, ParticleUnknown unknown) =>
        ParticleStartIndex + ParticleUnknownsCount * ParticleIndices[particleId] + (int)unknown;

    public int GetIndex(Guid nodeId, NodeUnknown unknown) => NodeStartIndex + NodeUnknownsCount * NodeIndices[nodeId] + (int)unknown;

    public double GetSolutionValue(Guid particleId, ParticleUnknown unknown) => _solution[GetIndex(particleId, unknown)];

    public double GetSolutionValue(Guid nodeId, NodeUnknown unknown) => _solution[GetIndex(nodeId, unknown)];

    public IReadOnlyList<double> Solution => _solution;

    public double[] EvaluateAt(double[] state)
    {
        var evaluation = YieldEquations(state).ToArray();

        if (evaluation.Any(x => !double.IsFinite(x)))
        {
            throw new InvalidOperationException("One ore more components of the gradient evaluated to an infinite value.");
        }

        return evaluation;
    }

    private IEnumerable<double> YieldEquations(double[] state) =>
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

    private IEnumerable<double> YieldStateVelocityDerivatives(double[] state)
    {
        foreach (var node in SolverSession.Nodes.Values)
        {
            // Normal Displacement
            var gibbsTerm = node.GibbsEnergyGradient.Normal * (1 + state[GetIndex(GlobalUnknown.Lambda1)]);
            var requiredConstraintsTerm = node.VolumeGradient.Normal * state[GetIndex(node.Id, NodeUnknown.Lambda2)];

            yield return gibbsTerm + requiredConstraintsTerm;
        }
    }

    private IEnumerable<double> YieldFluxDerivatives(double[] state)
    {
        foreach (var node in SolverSession.Nodes.Values) // for each flux
        {
            // Flux To Upper
            var dissipationTerm =
                -2 * SolverSession.GasConstant * SolverSession.Temperature * SolverSession.TimeStepWidth
              / (node.Particle.Material.MolarVolume * node.Particle.Material.EquilibriumVacancyConcentration)
              * node.SurfaceDiffusionCoefficient.ToUpper * node.SurfaceDistance.ToUpper * state[GetIndex(node.Id, NodeUnknown.FluxToUpper)]
              * state[GetIndex(GlobalUnknown.Lambda1)];
            var thisRequiredConstraintsTerm = SolverSession.TimeStepWidth * state[GetIndex(node.Id, NodeUnknown.Lambda2)];
            var upperRequiredConstraintsTerm = SolverSession.TimeStepWidth * state[GetIndex(node.Upper.Id, NodeUnknown.Lambda2)];

            yield return dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
        }
    }

    private IEnumerable<double> YieldDissipationEquality(double[] state)
    {
        var dissipation = SolverSession.Nodes.Values.Select(n =>
            n.GibbsEnergyGradient.Normal * state[GetIndex(n.Id, NodeUnknown.NormalDisplacement)]
        ).Sum();

        var dissipationFunction =
            SolverSession.GasConstant * SolverSession.Temperature * SolverSession.TimeStepWidth / 2
          * SolverSession.Nodes.Values.Select(n =>
                (
                    n.SurfaceDiffusionCoefficient.ToUpper * n.SurfaceDistance.ToUpper * Pow(state[GetIndex(n.Id, NodeUnknown.FluxToUpper)], 2)
                  + n.SurfaceDiffusionCoefficient.ToLower * n.SurfaceDistance.ToLower * Pow(state[GetIndex(n.Lower.Id, NodeUnknown.FluxToUpper)], 2)
                ) / (n.Particle.Material.MolarVolume * n.Particle.Material.EquilibriumVacancyConcentration)
            ).Sum();

        yield return dissipation - dissipationFunction;
    }

    private IEnumerable<double> YieldRequiredConstraints(double[] state)
    {
        foreach (var node in SolverSession.Nodes.Values)
        {
            var volumeTerm = node.VolumeGradient.Normal * state[GetIndex(node.Id, NodeUnknown.NormalDisplacement)];
            var fluxTerm =
                SolverSession.TimeStepWidth *
                (
                    state[GetIndex(node.Id, NodeUnknown.FluxToUpper)]
                  - state[GetIndex(node.Lower.Id, NodeUnknown.FluxToUpper)]
                );

            yield return volumeTerm - fluxTerm;
        }
    }

    public void FindRoot()
    {
        _solution = Broyden.FindRoot(
            EvaluateAt,
            initialGuess: _solution,
            maxIterations: SolverSession.Options.RootFindingMaxIterationCount,
            accuracy: SolverSession.Options.RootFindingAccuracy
        );
    }

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

        foreach (var particle in SolverSession.Particles.Values)
        {
        }
    }

    private IEnumerable<double> YieldNodeUnknownsInitialGuess()
    {
        foreach (var node in SolverSession.Nodes.Values)
        {
            yield return 1.0e-6;
            yield return 1.0e-4;
            yield return 1;
        }
    }
}