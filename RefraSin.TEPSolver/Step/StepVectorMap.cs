using MoreLinq;

namespace RefraSin.TEPSolver.Step;

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