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
    private Guid? _contactedParticleId;
    private Guid? _contactedNodeId;

    /// <inheritdoc />
    protected ContactNodeBase(INode node, Particle particle, ISolverSession solverSession) : base(node, particle, solverSession)
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

    protected ContactNodeBase(Guid id, double r, Angle phi, Particle particle, ISolverSession solverSession, Guid contactedNodeId,
        Guid contactedParticleId) :
        base(id, r, phi, particle, solverSession)
    {
        _contactedNodeId = contactedNodeId;
        _contactedParticleId = contactedParticleId;
    }

    /// <inheritdoc />
    public Guid ContactedParticleId => _contactedParticleId ??= SolverSession.CurrentState.AllNodes[ContactedNodeId].ParticleId;

    /// <inheritdoc />
    public Guid ContactedNodeId
    {
        get
        {
            if (_contactedNodeId.HasValue)
                return _contactedNodeId.Value;

            var error = SolverSession.Options.RelativeNodeCoordinateEquivalencePrecision * Particle.MeanRadius;

            var contactedNode =
                SolverSession.CurrentState.AllNodes.Values.FirstOrDefault(n =>
                    n.Id != Id && n.Coordinates.Absolute.Equals(Coordinates.Absolute, error)
                );

            _contactedNodeId = contactedNode?.Id ?? throw new InvalidOperationException("No corresponding node with same location could be found.");
            return _contactedNodeId.Value;
        }
    }
}