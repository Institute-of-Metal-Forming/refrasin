using RefraSin.Coordinates;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
/// </summary>
public abstract class ContactNodeBase<TContacted> : ContactNodeBase where TContacted : ContactNodeBase<TContacted>
{
    /// <inheritdoc />
    protected ContactNodeBase(INode node, Particle particle, ISolverSession solverSession) : base(node, particle, solverSession) { }

    protected ContactNodeBase(Guid id, double r, Angle phi, Particle particle, ISolverSession solverSession, Guid contactedNodeId,
        Guid contactedParticleId) :
        base(id, r, phi, particle, solverSession, contactedNodeId, contactedParticleId) { }

    /// <summary>
    /// Verbundener Knoten des anderen Partikels.
    /// </summary>
    public TContacted ContactedNode => _contactedNode ??=
        SolverSession.CurrentState.AllNodes[ContactedNodeId] as TContacted ??
        throw new InvalidCastException(
            $"Given contacted node {ContactedNodeId} does not refer to an instance of type {typeof(TContacted)}."
        );

    private TContacted? _contactedNode;

    /// <summary>
    /// Properties of the interface between two materials.
    /// </summary>
    public IMaterialInterface MaterialInterface => _materialInterface ??= Particle.MaterialInterfaces[ContactedNode.Particle.Material.Id];

    private IMaterialInterface? _materialInterface;

    /// <inheritdoc />
    public override double TransferCoefficient => MaterialInterface.TransferCoefficient;

    /// <summary>
    /// Stellt eine Verbindung zwischen zwei Knoten her (setzt die gegenseitigen <see cref="ContactedNode"/>).
    /// </summary>
    /// <param name="other">anderer Knoten</param>
    public virtual void Connect(TContacted other)
    {
        _contactedNode = other;
        other._contactedNode = (TContacted)this;
    }

    /// <summary>
    /// Löst eine Verbindung zwischen zwei Knoten (setzt die gegenseitigen <see cref="ContactedNode"/>).
    /// </summary>
    public virtual void Disconnect()
    {
        ContactedNode._contactedNode = null;
        _contactedNode = null;
    }

    public class NotConnectedException : InvalidOperationException
    {
        public NotConnectedException(ContactNodeBase<TContacted> sourceNode)
        {
            SourceNode = sourceNode;
            Message = $"Contact node {sourceNode} is not connected another node.";
        }

        public override string Message { get; }
        public ContactNodeBase<TContacted> SourceNode { get; }
    }
}

/// <summary>
/// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
/// </summary>
public abstract class ContactNodeBase : NodeBase, INodeContact
{
    /// <inheritdoc />
    protected ContactNodeBase(INode node, Particle particle, ISolverSession solverSession) : base(node, particle, solverSession)
    {
        if (node is INodeContact nodeContact)
        {
            ContactedNodeId = nodeContact.ContactedNodeId;
            ContactedParticleId = nodeContact.ContactedParticleId;
        }
        else
            throw new ArgumentException($"Given node does not implement {typeof(INodeContact)}.");
    }

    protected ContactNodeBase(Guid id, double r, Angle phi, Particle particle, ISolverSession solverSession, Guid contactedNodeId,
        Guid contactedParticleId) :
        base(id, r, phi, particle, solverSession)
    {
        ContactedNodeId = contactedNodeId;
        ContactedParticleId = contactedParticleId;
    }

    /// <inheritdoc />
    public Guid ContactedParticleId { get; }

    /// <inheritdoc />
    public Guid ContactedNodeId { get; }
}