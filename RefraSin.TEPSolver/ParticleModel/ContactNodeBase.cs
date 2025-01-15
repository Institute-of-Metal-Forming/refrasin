using RefraSin.Coordinates;
using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
/// </summary>
public abstract class ContactNodeBase<TContacted> : ContactNodeBase
    where TContacted : ContactNodeBase<TContacted>
{
    /// <inheritdoc />
    protected ContactNodeBase(INode node, Particle particle)
        : base(node, particle) { }

    protected ContactNodeBase(
        Guid id,
        double r,
        Angle phi,
        Particle particle,
        Guid contactedNodeId,
        Guid contactedParticleId
    )
        : base(id, r, phi, particle, contactedNodeId, contactedParticleId) { }

    /// <summary>
    /// Verbundener Knoten des anderen Partikels.
    /// </summary>
    public new TContacted ContactedNode =>
        _contactedNode ??=
            Particle.SolutionState.Nodes[ContactedNodeId] as TContacted
            ?? throw new InvalidCastException(
                $"Given contacted node {ContactedNodeId} does not refer to an instance of type {typeof(TContacted)}."
            );

    private TContacted? _contactedNode;

    /// <summary>
    /// Properties of the interface between two materials.
    /// </summary>
    public IInterfaceProperties InterfaceProperties =>
        _materialInterface ??= Particle.InterfaceProperties[ContactedNode.Particle.MaterialId];

    private IInterfaceProperties? _materialInterface;
}

/// <summary>
/// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
/// </summary>
public abstract class ContactNodeBase : NodeBase, INodeContact
{
    private Guid? _contactedParticleId;
    private Guid? _contactedNodeId;

    /// <inheritdoc />
    protected ContactNodeBase(INode node, Particle particle)
        : base(node, particle)
    {
        if (node is INodeContact nodeContact)
        {
            _contactedNodeId = nodeContact.ContactedNodeId;
            _contactedParticleId = nodeContact.ContactedParticleId;
        }
        else
        {
            _contactedNodeId = null;
            _contactedParticleId = null;
        }
    }

    protected ContactNodeBase(
        Guid id,
        double r,
        Angle phi,
        Particle particle,
        Guid contactedNodeId,
        Guid contactedParticleId
    )
        : base(id, r, phi, particle)
    {
        _contactedNodeId = contactedNodeId;
        _contactedParticleId = contactedParticleId;
    }

    /// <inheritdoc />
    public Guid ContactedParticleId =>
        _contactedParticleId ??= Particle.SolutionState.Nodes[ContactedNodeId].ParticleId;

    /// <inheritdoc />
    public Guid ContactedNodeId =>
        _contactedNodeId ??= Particle
            .SolutionState.NodeContacts.FromOrTo(Id)
            .Single()
            .Other(this)
            .Id;

    public ContactNodeBase ContactedNode =>
        _contactedNode ??=
            Particle.SolutionState.Nodes[ContactedNodeId] as ContactNodeBase
            ?? throw new InvalidCastException(
                $"Given contacted node {ContactedNodeId} does not refer to an instance of type {typeof(ContactNodeBase)}."
            );

    private ContactNodeBase? _contactedNode;
}
