using MoreLinq;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.Step;

public class StepVectorMap
{
    public StepVectorMap(IEnumerable<IParticle> particles, IEnumerable<INode> nodes)
    {
        GlobalUnknownsCount = Enum.GetNames(typeof(GlobalUnknown)).Length;

        ParticleUnknownsCount = Enum.GetNames(typeof(ParticleUnknown)).Length;
        Particles = particles as IParticle[] ?? particles.ToArray();
        ParticleCount = Particles.Length;
        ParticleStartIndex = GlobalUnknownsCount;
        ParticleIndices = Particles.Index().ToDictionary(kvp => kvp.Value.Id, kvp => kvp.Key);

        NodeUnknownsCount = Enum.GetNames(typeof(NodeUnknown)).Length;
        Nodes = nodes as INode[] ?? nodes.ToArray();
        NodeCount = Nodes.Length;
        NodeStartIndex = ParticleStartIndex + ParticleCount * ParticleUnknownsCount;
        NodeIndices = Nodes.Index().ToDictionary(kvp => kvp.Value.Id, kvp => kvp.Key);

        TotalUnknownsCount = NodeStartIndex + NodeCount * NodeUnknownsCount;
    }

    public INode[] Nodes { get; }

    public IParticle[] Particles { get; }

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