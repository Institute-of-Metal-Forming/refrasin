using MoreLinq;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.Step;

internal class StepVectorMap
{
    public StepVectorMap(IEnumerable<IParticleSpec> particles, IEnumerable<INodeSpec> nodes)
    {
        GlobalUnknownsCount = Enum.GetNames(typeof(GlobalUnknown)).Length;

        ParticleUnknownsCount = Enum.GetNames(typeof(ParticleUnknown)).Length;
        var particleArray = particles as IParticleSpec[] ?? particles.ToArray();
        ParticleCount = particleArray.Length;
        ParticleStartIndex = GlobalUnknownsCount;
        ParticleIndices = particleArray.Index().ToDictionary(kvp => kvp.Value.Id, kvp => kvp.Key);

        NodeUnknownsCount = Enum.GetNames(typeof(NodeUnknown)).Length;
        var nodeArray = nodes as INodeSpec[] ?? nodes.ToArray();
        NodeCount = nodeArray.Length;
        NodeStartIndex = ParticleStartIndex + ParticleCount * ParticleUnknownsCount;
        NodeIndices = nodeArray.Index().ToDictionary(kvp => kvp.Value.Id, kvp => kvp.Key);

        TotalUnknownsCount = NodeStartIndex + NodeCount * NodeUnknownsCount;
    }

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