using MathNet.Numerics.RootFinding;
using MoreLinq;

namespace RefraSin.TEPSolver;

internal class LagrangianGradient
{
    public LagrangianGradient(ISolverSession solverSession)
    {
        SolverSession = solverSession;

        ParticleCount = solverSession.Particles.Count;
        ParticleStartIndex = 1;
        ParticleIndices = solverSession.Particles.Keys.Index().ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        NodeCount = solverSession.Nodes.Count;
        NodeStartIndex = ParticleStartIndex + ParticleCount * Enum.GetNames(typeof(ParticleUnknown)).Length;
        NodeIndices = solverSession.Nodes.Keys.Index().ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        TotalUnknownsCount = NodeStartIndex + NodeCount * Enum.GetNames(typeof(NodeUnknown)).Length + 1;

        _solution = Enumerable.Repeat(1.0, TotalUnknownsCount).ToArray();
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

    public enum NodeUnknown
    {
        NormalDisplacement,
        FluxToUpper,
    }

    public enum ParticleUnknown
    {
        // RadialDisplacement,
        // AngleDisplacement,
        // RotationDisplacement
    }

    public int GetIndex(Guid particleId, ParticleUnknown unknown) => ParticleStartIndex + (int)unknown * ParticleIndices[particleId];

    public int GetIndex(Guid nodeId, NodeUnknown unknown) => NodeStartIndex + (int)unknown * NodeIndices[nodeId];

    public double GetSolutionValue(Guid particleId, ParticleUnknown unknown) => _solution[GetIndex(particleId, unknown)];

    public double GetSolutionValue(Guid nodeId, NodeUnknown unknown) => _solution[GetIndex(nodeId, unknown)];

    public IReadOnlyList<double> Solution => _solution;

    public double[] EvaluateAt(double[] state)
    {
        throw new NotImplementedException();
    }

    public void FindRoot()
    {
        _solution = Broyden.FindRoot(EvaluateAt, _solution);
    }
}