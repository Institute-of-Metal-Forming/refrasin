using MoreLinq;
using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver.StepVectors;

public class StepVectorMap
{
    public StepVectorMap(IEnumerable<Particle> particles, IEnumerable<NodeBase> nodes)
    {
        GlobalUnknownsCount = Enum.GetNames(typeof(GlobalUnknown)).Length;

        ParticleUnknownsCount = Enum.GetNames(typeof(ParticleUnknown)).Length;
        Particles = particles as Particle[] ?? particles.ToArray();
        ParticleCount = Particles.Length;
        ParticleStartIndex = GlobalUnknownsCount;
        ParticleIndices = Particles.Index().ToDictionary(kvp => kvp.Value.Id, kvp => kvp.Key);

        NodeUnknownsCount = Enum.GetNames(typeof(NodeUnknown)).Length;
        Nodes = nodes as NodeBase[] ?? nodes.ToArray();
        NodeCount = Nodes.Length;
        NodeStartIndex = ParticleStartIndex + ParticleCount * ParticleUnknownsCount;
        NodeIndices = Nodes.Index().ToDictionary(kvp => kvp.Value.Id, kvp => kvp.Key);

        TotalUnknownsCount = NodeStartIndex + NodeCount * NodeUnknownsCount;
    }

    public NodeBase[] Nodes { get; }

    public Particle[] Particles { get; }

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

    internal int GetIndex(GlobalUnknown unknown) => (int)unknown;

    internal int GetIndex(Guid particleId, ParticleUnknown unknown) =>
        ParticleStartIndex + ParticleUnknownsCount * ParticleIndices[particleId] + (int)unknown;

    internal int GetIndex(Guid nodeId, NodeUnknown unknown) => NodeStartIndex + NodeUnknownsCount * NodeIndices[nodeId] + (int)unknown;
}