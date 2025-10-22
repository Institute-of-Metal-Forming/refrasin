using RefraSin.Coordinates;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Nodes.Extensions;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
/// </summary>
public abstract class ContactNodeBase<TContacted> : ContactNodeBase
    where TContacted : ContactNodeBase<TContacted>
{
    /// <inheritdoc />
    protected ContactNodeBase(
        INode node,
        Particle particle,
        Guid? contactedNodeId = null,
        Guid? contactedParticleId = null
    )
        : base(node, particle, contactedNodeId, contactedParticleId) { }

    protected ContactNodeBase(
        Guid id,
        double r,
        Angle phi,
        Particle particle,
        Guid? contactedNodeId = null,
        Guid? contactedParticleId = null
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
}

/// <summary>
/// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
/// </summary>
public abstract class ContactNodeBase : NodeBase, INodeContact
{
    /// <inheritdoc />
    protected ContactNodeBase(
        INode node,
        Particle particle,
        Guid? contactedNodeId = null,
        Guid? contactedParticleId = null
    )
        : base(node, particle)
    {
        _contactedNodeId = contactedNodeId;
        _contactedParticleId = contactedParticleId;
    }

    protected ContactNodeBase(
        Guid id,
        double r,
        Angle phi,
        Particle particle,
        Guid? contactedNodeId = null,
        Guid? contactedParticleId = null
    )
        : base(id, r, phi, particle)
    {
        _contactedNodeId = contactedNodeId;
        _contactedParticleId = contactedParticleId;
    }

    /// <inheritdoc />
    public Guid ContactedParticleId => _contactedParticleId ?? ContactedNode.Particle.Id;

    /// <inheritdoc />
    public Guid ContactedNodeId =>
        _contactedNodeId ?? this.FindContactedNodeByCoordinates(Particle.SolutionState.Nodes).Id;

    public ContactNodeBase ContactedNode =>
        _contactedNode ??=
            Particle.SolutionState.Nodes[ContactedNodeId] as ContactNodeBase
            ?? throw new InvalidCastException(
                $"Given contacted node {ContactedNodeId} does not refer to an instance of type {typeof(ContactNodeBase)}."
            );

    private ContactNodeBase? _contactedNode;
    private readonly Guid? _contactedParticleId;
    private readonly Guid? _contactedNodeId;
}
