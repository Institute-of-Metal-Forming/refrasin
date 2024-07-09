using RefraSin.Coordinates;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using static System.Math;
using static MathNet.Numerics.Constants;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
/// </summary>
public abstract class ContactNodeBase<TContacted> : ContactNodeBase
    where TContacted : ContactNodeBase<TContacted>
{
    /// <inheritdoc />
    protected ContactNodeBase(INode node, Particle particle, ISolverSession solverSession)
        : base(node, particle, solverSession) { }

    protected ContactNodeBase(
        Guid id,
        double r,
        Angle phi,
        Particle particle,
        ISolverSession solverSession,
        Guid contactedNodeId,
        Guid contactedParticleId
    )
        : base(id, r, phi, particle, solverSession, contactedNodeId, contactedParticleId) { }

    /// <summary>
    /// Verbundener Knoten des anderen Partikels.
    /// </summary>
    public new TContacted ContactedNode =>
        _contactedNode ??=
            SolverSession.CurrentState.Nodes[ContactedNodeId] as TContacted
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

    /// <inheritdoc />
    public override NormalTangential<Angle> CenterShiftVectorDirection =>
        _centerShiftVectorDirection ??= IsParentsNode
            ? new NormalTangential<Angle>(
                Pi
              - (Coordinates.Phi - ContactDirection).Reduce(Angle.ReductionDomain.WithNegative)
              + (Pi - SurfaceNormalAngle.ToLower - SurfaceRadiusAngle.ToLower),
                Pi
              - (Coordinates.Phi - ContactDirection).Reduce(Angle.ReductionDomain.WithNegative)
              - (Pi - SurfaceTangentAngle.ToUpper - SurfaceRadiusAngle.ToUpper)
            )
            : new NormalTangential<Angle>(
                Pi
              + (ContactDirection - Coordinates.Phi).Reduce(Angle.ReductionDomain.WithNegative)
              - (Pi - SurfaceNormalAngle.ToUpper - SurfaceRadiusAngle.ToUpper),
                Pi
              + (ContactDirection - Coordinates.Phi).Reduce(Angle.ReductionDomain.WithNegative)
              - (Pi - SurfaceTangentAngle.ToUpper - SurfaceRadiusAngle.ToUpper)
            );

    private NormalTangential<Angle>? _centerShiftVectorDirection;

    /// <inheritdoc />
    public override NormalTangential<double> ContactDistanceGradient =>
        _contactDistanceGradient ??= new NormalTangential<double>(
            -Cos(CenterShiftVectorDirection.Normal),
            -Cos(CenterShiftVectorDirection.Tangential)
        );

    private NormalTangential<double>? _contactDistanceGradient;

    /// <inheritdoc />
    public override NormalTangential<double> ContactDirectionGradient =>
        _contactDirectionGradient ??= new NormalTangential<double>(
            Sin(CenterShiftVectorDirection.Normal) / ContactDistance,
            Sin(CenterShiftVectorDirection.Tangential) / ContactDistance
        );

    private NormalTangential<double>? _contactDirectionGradient;
}

/// <summary>
/// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
/// </summary>
public abstract class ContactNodeBase : NodeBase, IContactNode
{
    private Guid? _contactedParticleId;
    private Guid? _contactedNodeId;

    /// <inheritdoc />
    protected ContactNodeBase(INode node, Particle particle, ISolverSession solverSession)
        : base(node, particle, solverSession)
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
        ISolverSession solverSession,
        Guid contactedNodeId,
        Guid contactedParticleId
    )
        : base(id, r, phi, particle, solverSession)
    {
        _contactedNodeId = contactedNodeId;
        _contactedParticleId = contactedParticleId;
    }

    /// <inheritdoc />
    public Guid ContactedParticleId =>
        _contactedParticleId ??= SolverSession.CurrentState.Nodes[ContactedNodeId].ParticleId;

    /// <inheritdoc />
    public Guid ContactedNodeId => _contactedNodeId ??= SolverSession.CurrentState.NodeContacts[Id];

    public ContactNodeBase ContactedNode =>
        _contactedNode ??=
            SolverSession.CurrentState.Nodes[ContactedNodeId] as ContactNodeBase
         ?? throw new InvalidCastException(
                $"Given contacted node {ContactedNodeId} does not refer to an instance of type {typeof(ContactNodeBase)}."
            );

    private ContactNodeBase? _contactedNode;

    /// <inheritdoc />
    public double ContactDistance => Contact.Distance;

    /// <inheritdoc />
    public Angle ContactDirection => IsParentsNode ? Contact.DirectionFrom : Contact.DirectionTo;

    /// <inheritdoc />
    public Angle AngleDistanceFromContactDirection =>
        _angleDistanceFromContactDirection ??= (Coordinates.Phi - ContactDirection).Reduce(
            Angle.ReductionDomain.WithNegative
        );

    private Angle? _angleDistanceFromContactDirection;

    /// <inheritdoc />
    public abstract NormalTangential<Angle> CenterShiftVectorDirection { get; }

    /// <inheritdoc />
    public abstract NormalTangential<double> ContactDistanceGradient { get; }

    /// <inheritdoc />
    public abstract NormalTangential<double> ContactDirectionGradient { get; }

    public bool IsParentsNode => _isParentsNode ??= Contact.From.Id == ParticleId;
    private bool? _isParentsNode;

    public ParticleContact Contact
    {
        get
        {
            if (_contact is not null)
                return _contact;

            _contact = SolverSession.CurrentState.ParticleContacts.FirstOrDefault(c =>
                c.From.Id == ParticleId && c.To.Id == ContactedParticleId
            );

            if (_contact is not null)
                return _contact;

            _contact = SolverSession.CurrentState.ParticleContacts.FirstOrDefault(c =>
                c.To.Id == ParticleId && c.From.Id == ContactedParticleId
            );

            if (_contact is not null)
                return _contact;

            throw new InvalidOperationException("Related contact was not found.");
        }
    }

    private ParticleContact? _contact;
}