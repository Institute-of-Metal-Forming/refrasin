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
        Coordinates = new PolarPoint(node.Coordinates.ToTuple()) { SystemSource = () => Particle.LocalCoordinateSystem };
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
    public ToUpperToLowerAngle AngleDistance => _angleDistance ??= new ToUpperToLowerAngle(
        Coordinates.AngleTo(Upper.Coordinates),
        Coordinates.AngleTo(Lower.Coordinates)
    );

    private ToUpperToLowerAngle? _angleDistance;

    /// <summary>
    ///     Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
    /// </summary>
    public ToUpperToLower SurfaceDistance => _surfaceDistance ??= new ToUpperToLower(
        CosLaw.C(Upper.Coordinates.R, Coordinates.R, AngleDistance.ToUpper),
        CosLaw.C(Lower.Coordinates.R, Coordinates.R, AngleDistance.ToLower)
    );

    private ToUpperToLower? _surfaceDistance;

    /// <summary>
    ///     Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
    /// </summary>
    public ToUpperToLowerAngle SurfaceRadiusAngle => _surfaceRadiusAngle ??= new ToUpperToLowerAngle(
        CosLaw.Gamma(SurfaceDistance.ToUpper, Coordinates.R, Upper.Coordinates.R),
        CosLaw.Gamma(SurfaceDistance.ToLower, Coordinates.R, Lower.Coordinates.R)
    );

    private ToUpperToLowerAngle? _surfaceRadiusAngle;

    /// <summary>
    ///     Gesamtes Volumen der an den Knoten angrenzenden Elemente.
    /// </summary>
    public ToUpperToLower Volume => _volume ??= new ToUpperToLower(
        0.5 * Coordinates.R * Upper.Coordinates.R * Sin(AngleDistance.ToUpper),
        0.5 * Coordinates.R * Lower.Coordinates.R * Sin(AngleDistance.ToLower)
    );

    private ToUpperToLower? _volume;

    public NormalTangentialAngle SurfaceVectorAngle => _surfaceAngle ??= new NormalTangentialAngle(
        PI - 0.5 * SurfaceRadiusAngle.Sum,
        PI / 2 - 0.5 * SurfaceRadiusAngle.Sum
    );

    private NormalTangentialAngle? _surfaceAngle;

    /// <inheritdoc />
    public abstract ToUpperToLower SurfaceEnergy { get; }

    /// <inheritdoc />
    public abstract ToUpperToLower SurfaceDiffusionCoefficient { get; }

    /// <inheritdoc />
    public abstract double TransferCoefficient { get; }

    /// <inheritdoc />
    public NormalTangential GibbsEnergyGradient => _gibbsEnergyGradient ??= new NormalTangential(
        -(SurfaceEnergy.ToUpper + SurfaceEnergy.ToLower) * Cos(SurfaceVectorAngle.Normal),
        -(SurfaceEnergy.ToUpper - SurfaceEnergy.ToLower) * Cos(SurfaceVectorAngle.Tangential)
    );

    private NormalTangential? _gibbsEnergyGradient;

    /// <inheritdoc />
    public NormalTangential VolumeGradient => _volumeGradient ??= new NormalTangential(
        0.5 * (SurfaceDistance.ToUpper + SurfaceDistance.ToLower) * Sin(SurfaceVectorAngle.Normal),
        0.5 * (SurfaceDistance.ToUpper - SurfaceDistance.ToLower) * Sin(SurfaceVectorAngle.Tangential)
    );

    private NormalTangential? _volumeGradient;

    public double GuessFluxToUpper()
    {
        var x1 = -Lower.Coordinates.R * Sin(AngleDistance.ToLower);
        var x3 = Upper.Coordinates.R * Sin(AngleDistance.ToUpper);
        var y1 = Lower.Coordinates.R * Cos(AngleDistance.ToLower);
        var y2 = Coordinates.R;
        var y3 = Upper.Coordinates.R * Cos(AngleDistance.ToUpper);

        var curvature = -(x3 * y1 + x1 * y2 - x3 * y2 - x1 * y3) / (Pow(x1, 2) * x3 - x1 * Pow(x3, 2));
        var curvatureGibbs = GibbsEnergyGradient.Normal / SurfaceDistance.Sum / SurfaceEnergy.ToUpper;

        var vacancyConcentrationGradient = -Particle.Material.EquilibriumVacancyConcentration
                                         / (SolverSession.GasConstant * SolverSession.Temperature)
                                         * (Upper.GibbsEnergyGradient.Normal - GibbsEnergyGradient.Normal)
                                         * Particle.Material.MolarVolume
                                         / Pow(SurfaceDistance.ToUpper, 2);
        return -SurfaceDiffusionCoefficient.ToUpper * vacancyConcentrationGradient;
    }

    public double GuessNormalDisplacement()
    {
        var fluxBalance = GuessFluxToUpper() - Lower.GuessFluxToUpper();

        var displacement = 2 * fluxBalance / (SurfaceDistance.Sum * Sin(SurfaceVectorAngle.Normal));
        return displacement;
    }

    public abstract NodeBase ApplyTimeStep(StepVector stepVector, double timeStepWidth, Particle particle);

    public override string ToString() =>
        $"{GetType().Name} {Id} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)} of {Particle}";
}