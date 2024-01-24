using MoreLinq;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;
using ParticleContact = RefraSin.TEPSolver.ParticleModel.ParticleContact;

namespace RefraSin.TEPSolver.StepVectors;

public class StepVectorMap
{
    public StepVectorMap(IEnumerable<IParticleContact> contacts, IEnumerable<INode> nodes)
    {
        GlobalUnknownsCount = Enum.GetNames(typeof(GlobalUnknown)).Length;

        ContactUnknownsCount = Enum.GetNames(typeof(ContactUnknown)).Length;
        var contactsArray = contacts as ParticleContact[] ?? contacts.ToArray();
        ContactCount = contactsArray.Length;
        ContactStartIndex = GlobalUnknownsCount;
        ContactIndices = contactsArray.Index().ToDictionary(
            kvp => (kvp.Value.From.Id, kvp.Value.To.Id),
            kvp => kvp.Key
        );

        NodeUnknownsCount = Enum.GetNames(typeof(NodeUnknown)).Length;
        var nodesArray = nodes as NodeBase[] ?? nodes.ToArray();
        NodeCount = nodesArray.Length;
        NodeStartIndex = ContactStartIndex + ContactCount * ContactUnknownsCount;
        NodeIndices = nodesArray.Index().ToDictionary(kvp => kvp.Value.Id, kvp => kvp.Key);

        ContactNodeUnknownsCount = Enum.GetNames(typeof(ContactNodeUnknown)).Length;
        var contactNodesArray = nodesArray.OfType<ContactNodeBase>().ToArray();
        ContactNodeCount = contactNodesArray.Length;
        ContactNodeStartIndex = NodeStartIndex + NodeCount * NodeUnknownsCount;
        ContactNodeIndices = contactNodesArray.Index().ToDictionary(kvp => kvp.Value.Id, kvp => kvp.Key);

        TotalUnknownsCount = ContactNodeStartIndex + ContactNodeCount * ContactNodeUnknownsCount;
    }

    public int ContactCount { get; }

    public int ContactStartIndex { get; }

    public IReadOnlyDictionary<(Guid from, Guid to), int> ContactIndices { get; }

    public int NodeCount { get; }

    public int NodeStartIndex { get; }

    public IReadOnlyDictionary<Guid, int> NodeIndices { get; }

    public int ContactNodeCount { get; }

    public int ContactNodeStartIndex { get; }

    public Dictionary<Guid, int> ContactNodeIndices { get; }

    public int TotalUnknownsCount { get; }

    public int GlobalUnknownsCount { get; }

    public int ContactUnknownsCount { get; }

    public int NodeUnknownsCount { get; }

    public int ContactNodeUnknownsCount { get; }

    internal int GetIndex(GlobalUnknown unknown) => (int)unknown;

    internal int GetIndex(Guid fromParticleId, Guid toParticleId, ContactUnknown unknown) =>
        ContactStartIndex + ContactUnknownsCount * ContactIndices[(fromParticleId, toParticleId)] + (int)unknown;

    internal int GetIndex(Guid nodeId, NodeUnknown unknown) => NodeStartIndex + NodeUnknownsCount * NodeIndices[nodeId] + (int)unknown;
    
    internal int GetIndex(Guid nodeId, ContactNodeUnknown unknown) => ContactNodeStartIndex + ContactNodeUnknownsCount * ContactNodeIndices[nodeId] + (int)unknown;
}