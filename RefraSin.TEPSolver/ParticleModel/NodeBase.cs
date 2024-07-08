using System.Globalization;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.Enumerables;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.Exceptions;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using static MathNet.Numerics.Constants;
using static RefraSin.Coordinates.Angle.ReductionDomain;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Abstract base class for particle surface nodes.
/// </summary>
public abstract class NodeBase : INode, INodeGeometry, INodeGradients, INodeMaterialProperties
{
    protected NodeBase(INode node, Particle particle, ISolverSession solverSession)
    {
        Id = node.Id;

        if (node.ParticleId != particle.Id)
            throw new ArgumentException("IDs of the node spec and the given particle instance do not match.");

        Particle = particle;
        Coordinates = new PolarPoint(node.Coordinates.Phi, node.Coordinates.R / solverSession.Norm.Length)
            { SystemSource = () => Particle.LocalCoordinateSystem };
        SolverSession = solverSession;
    }

    protected NodeBase(Guid id, double r, Angle phi, Particle particle, ISolverSession solverSession)
    {
        Id = id;
        Particle = particle;
        Coordinates = new PolarPoint(phi.Reduce(AllPositive), r, Particle.LocalCoordinateSystem);
        SolverSession = solverSession;
    }

    /// <summary>
    /// Reference to the current solver session.
    /// </summary>
    protected ISolverSession SolverSession { get; }

    public Guid Id { get; set; }

    /// <summary>
    ///     Partikel, zu dem dieser Knoten gehört.
    /// </summary>
    public Particle Particle { get; }

    IParticle INodeNeighbors.Particle => Particle;

    /// <inheritdoc />
    public Guid ParticleId => Particle.Id;

    public int Index => _index ??= Particle.Nodes.IndexOf(Id);

    private int? _index;

    /// <summary>
    /// A reference to the upper neighbor of this node.
    /// </summary>
    /// <exception cref="InvalidNeighborhoodException">If this node has currently no upper neighbor set.</exception>
    public NodeBase Upper => _upper ??= Particle.Nodes[Index + 1];

    private NodeBase? _upper;

    /// <summary>
    /// A reference to the lower neighbor of this node.
    /// </summary>
    /// <exception cref="InvalidNeighborhoodException">If this node has currently no lower neighbor set.</exception>
    public NodeBase Lower => _lower ??= Particle.Nodes[Index - 1];

    private NodeBase? _lower;

    /// <summary>
    /// Coordinates of the node in terms of particle's local coordinate system <see cref="ParticleModel.Particle.LocalCoordinateSystem" />
    /// </summary>
    public PolarPoint Coordinates { get; }

    /// <inheritdoc />
    public abstract NodeType Type { get; }

    /// <inheritdoc />
    public AbsolutePoint AbsoluteCoordinates => Coordinates.Absolute;

    /// <summary>
    ///     Winkeldistanz zu den Nachbarknoten (Größe des kürzesten Winkels).
    /// </summary>
    public ToUpperToLower<Angle> AngleDistance => _angleDistance ??= this.AngleDistance();

    private ToUpperToLower<Angle>? _angleDistance;

    /// <summary>
    ///     Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
    /// </summary>
    public ToUpperToLower<double> SurfaceDistance => _surfaceDistance ??= this.SurfaceDistance();

    private ToUpperToLower<double>? _surfaceDistance;

    /// <summary>
    ///     Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
    /// </summary>
    public ToUpperToLower<Angle> SurfaceRadiusAngle => _surfaceRadiusAngle ??= this.SurfaceRadiusAngle();

    private ToUpperToLower<Angle>? _surfaceRadiusAngle;

    /// <summary>
    ///     Gesamtes Volumen der an den Knoten angrenzenden Elemente.
    /// </summary>
    public ToUpperToLower<double> Volume => _volume ??= this.Volume();
    
    private ToUpperToLower<double>? _volume;

    public virtual ToUpperToLower<Angle> SurfaceNormalAngle => _surfaceNormalAngle ??= this.SurfaceNormalAngle();

    private ToUpperToLower<Angle>? _surfaceNormalAngle;

    public virtual ToUpperToLower<Angle> SurfaceTangentAngle => _surfaceTangentAngle ??= this.SurfaceTangentAngle();

    private ToUpperToLower<Angle>? _surfaceTangentAngle;

    /// <inheritdoc />
    public abstract ToUpperToLower<double> SurfaceEnergy { get; }

    /// <inheritdoc />
    public abstract ToUpperToLower<double> SurfaceDiffusionCoefficient { get; }

    /// <inheritdoc />
    public NormalTangential<double> GibbsEnergyGradient => _gibbsEnergyGradient ??= new NormalTangential<double>(
        -(SurfaceEnergy.ToUpper * Cos(SurfaceNormalAngle.ToUpper) + SurfaceEnergy.ToLower * Cos(SurfaceNormalAngle.ToLower)),
        -(SurfaceEnergy.ToUpper * Cos(SurfaceTangentAngle.ToUpper) - SurfaceEnergy.ToLower * Cos(SurfaceTangentAngle.ToLower))
    );

    private NormalTangential<double>? _gibbsEnergyGradient;

    /// <inheritdoc />
    public NormalTangential<double> VolumeGradient => _volumeGradient ??= new NormalTangential<double>(
        0.5 * (SurfaceDistance.ToUpper * Sin(SurfaceNormalAngle.ToUpper) + SurfaceDistance.ToLower * Sin(SurfaceNormalAngle.ToLower)),
        0.5 * (SurfaceDistance.ToUpper * Sin(SurfaceTangentAngle.ToUpper) - SurfaceDistance.ToLower * Sin(SurfaceTangentAngle.ToLower))
    );

    private NormalTangential<double>? _volumeGradient;

    public abstract NodeBase ApplyTimeStep(StepVector stepVector, double timeStepWidth, Particle particle);

    public override string ToString() =>
        $"{GetType().Name} {Id} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)} of {Particle}";
}