using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
/// </summary>
internal abstract class ContactNode<TContacted> : ContactNode where TContacted : ContactNode<TContacted>
{
    /// <inheritdoc />
    protected ContactNode(INode node, Particle particle, ISolverSession solverSession) : base(node, particle, solverSession) { }

    private TContacted? _contactedNode = null;

    /// <summary>
    /// Verbundener Knoten des anderen Partikels.
    /// </summary>
    public TContacted ContactedNode => _contactedNode ?? throw new NotConnectedException(this);

    /// <summary>
    /// Properties of the interface between two materials.
    /// </summary>
    public IMaterialInterface MaterialInterface => _materialInterface ??= Particle.MaterialInterfaces[ContactedNode.Particle.Material.Id];

    private IMaterialInterface? _materialInterface;

    /// <inheritdoc />
    public override Guid ContactedParticleId => ContactedNode.ParticleId;

    /// <inheritdoc />
    public override Guid ContactedNodeId => ContactedNode.Id;

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
        public NotConnectedException(ContactNode<TContacted> sourceNode)
        {
            SourceNode = sourceNode;
            Message = $"Contact node {sourceNode} is not connected another node.";
        }

        public override string Message { get; }
        public ContactNode<TContacted> SourceNode { get; }
    }
}

/// <summary>
/// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
/// </summary>
internal abstract class ContactNode : Node, IContactNode
{
    /// <inheritdoc />
    protected ContactNode(INode node, Particle particle, ISolverSession solverSession) : base(node, particle, solverSession) { }

    /// <inheritdoc />
    public abstract Guid ContactedParticleId { get; }

    /// <inheritdoc />
    public abstract Guid ContactedNodeId { get; }

    /// <inheritdoc />
    protected override void CheckState(INode state)
    {
        base.CheckState(state);

        if (state is IContactNode contactState)
        {
            if (contactState.ContactedParticleId != ContactedParticleId)
                throw new InvalidOperationException("Contacted particle IDs of state and node do not match.");
            if (contactState.ContactedNodeId != ContactedNodeId)
                throw new InvalidOperationException("Contacted node IDs of state and node do not match.");
        }
        else
        {
            throw new ArgumentException($"The given state is no instance of {nameof(IContactNode)}", nameof(state));
        }
    }
}