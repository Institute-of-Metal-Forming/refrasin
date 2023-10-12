using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.RootFinding;
using MathNet.Numerics.LinearAlgebra.Double;
using MoreLinq;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;
using static System.Math;

namespace RefraSin.TEPSolver;

internal class StepVectorMap
{
    public StepVectorMap(ISolverSession solverSession)
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
    }

    public ISolverSession SolverSession { get; }

    public int ParticleCount { get; }

    public int ParticleStartIndex { get; }

    public IReadOnlyDictionary<Guid, int> ParticleIndices { get; }

    public int NodeCount { get; }

    public int NodeStartIndex { get; }

    public IReadOnlyDictionary<Guid, int> NodeIndices { get; }

    public int TotalUnknownsCount { get; }

    public int GlobalUnknownsCount { get; }

    public int ParticleUnknownsCount { get; }

    public int NodeUnknownsCount { get; }

    public int GetIndex(GlobalUnknown unknown) => (int)unknown;

    public int GetIndex(Guid particleId, ParticleUnknown unknown) =>
        ParticleStartIndex + ParticleUnknownsCount * ParticleIndices[particleId] + (int)unknown;

    public int GetIndex(Guid nodeId, NodeUnknown unknown) => NodeStartIndex + NodeUnknownsCount * NodeIndices[nodeId] + (int)unknown;
}

public enum GlobalUnknown
{
    Lambda1
}

public enum NodeUnknown
{
    NormalDisplacement,
    FluxToUpper,
    Lambda2
}

public enum ParticleUnknown
{
    // RadialDisplacement,
    // AngleDisplacement,
    // RotationDisplacement
}

internal class StepVector : DenseVector
{
    /// <inheritdoc />
    public StepVector(double[] storage, StepVectorMap stepVectorMap) : base(storage)
    {
        StepVectorMap = stepVectorMap;
    }

    /// <inheritdoc />
    public StepVector(Vector<double> vector, StepVectorMap stepVectorMap) : base(vector.AsArray() ?? vector.ToArray())
    {
        StepVectorMap = stepVectorMap;
    }

    public StepVectorMap StepVectorMap { get; }

    public NodeView this[INodeSpec node] => new(this, node.Id);

    public ParticleView this[IParticleSpec particle] => new(this, particle.Id);

    public double Lambda1 => this[StepVectorMap.GetIndex(GlobalUnknown.Lambda1)];
}

internal class ParticleView
{
    private readonly StepVector _vector;
    private readonly Guid _particleId;

    public ParticleView(StepVector vector, Guid particleId)
    {
        _vector = vector;
        _particleId = particleId;
    }
}

internal class NodeView
{
    private readonly StepVector _vector;
    private readonly Guid _nodeId;

    public NodeView(StepVector vector, Guid nodeId)
    {
        _vector = vector;
        _nodeId = nodeId;
    }

    public double NormalDisplacement => _vector[_vector.StepVectorMap.GetIndex(_nodeId, NodeUnknown.NormalDisplacement)];
    public double FluxToUpper => _vector[_vector.StepVectorMap.GetIndex(_nodeId, NodeUnknown.FluxToUpper)];
    public double Lambda2 => _vector[_vector.StepVectorMap.GetIndex(_nodeId, NodeUnknown.Lambda2)];
}

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

    private Vector<double> EvaluateAtArray(Vector<double> state) => EvaluateAt(new StepVector(state, StepVectorMap));

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

    public void FindRoot()
    {
        try
        {
            Solution = new StepVector(Broyden.FindRoot(
                EvaluateAtArray,
                initialGuess: Solution,
                maxIterations: SolverSession.Options.RootFindingMaxIterationCount,
                accuracy: SolverSession.Options.RootFindingAccuracy
            ), StepVectorMap);
        }
        catch (NonConvergenceException e)
        {
            Solution = new StepVector(Broyden.FindRoot(
                EvaluateAtArray,
                initialGuess: GuessSolution(),
                maxIterations: SolverSession.Options.RootFindingMaxIterationCount,
                accuracy: SolverSession.Options.RootFindingAccuracy
            ), StepVectorMap);
        }
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