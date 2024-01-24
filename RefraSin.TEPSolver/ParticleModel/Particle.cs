using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Stellt ein Pulverpartikel dar.
/// </summary>
public class Particle : IParticle
{
    private ReadOnlyNodeCollection<NodeBase> _nodes;
    private double? _meanRadius;

    public Particle(
        IParticle particle,
        ISolverSession solverSession
    )
    {
        Id = particle.Id;
        CenterCoordinates = particle.CenterCoordinates.Clone();
        RotationAngle = particle.RotationAngle;

        LocalCoordinateSystem = new PolarCoordinateSystem
        {
            OriginSource = () => CenterCoordinates,
            RotationAngleSource = () => RotationAngle
        };

        Material = solverSession.MaterialRegistry.GetMaterial(particle.MaterialId);
        MaterialInterfaces = solverSession.MaterialRegistry.MaterialInterfaces
            .Where(i => i.From == particle.MaterialId)
            .ToDictionary(i => i.To);

        SolverSession = solverSession;
        _nodes = particle.Nodes.Select(node => node switch
        {
            INeckNode neckNode                   => new NeckNode(neckNode, this, solverSession),
            IGrainBoundaryNode grainBoundaryNode => new GrainBoundaryNode(grainBoundaryNode, this, solverSession),
            { Type: NodeType.GrainBoundaryNode } => new GrainBoundaryNode(node, this, solverSession),
            { Type: NodeType.NeckNode }          => new NeckNode(node, this, solverSession),
            _                                    => (NodeBase)new SurfaceNode(node, this, solverSession),
        }).ToReadOnlyNodeCollection();
    }

    private Particle(
        Guid id,
        AbsolutePoint centerCoordinates,
        Angle rotationAngle,
        IMaterial material,
        IReadOnlyDictionary<Guid, IMaterialInterface> materialInterfaces,
        ISolverSession solverSession
    )
    {
        Id = id;
        CenterCoordinates = centerCoordinates;
        RotationAngle = rotationAngle;

        LocalCoordinateSystem = new PolarCoordinateSystem
        {
            OriginSource = () => CenterCoordinates,
            RotationAngleSource = () => RotationAngle
        };

        Material = material;
        MaterialInterfaces = materialInterfaces;

        SolverSession = solverSession;
        _nodes = ReadOnlyNodeCollection<NodeBase>.Empty;
    }

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <summary>
    /// Material data.
    /// </summary>
    public IMaterial Material { get; }

    /// <inheritdoc />
    Guid IParticle.MaterialId => Material.Id;

    /// <summary>
    /// Dictionary of material IDs to material interface data, assuming that the current instances material is always on the from side.
    /// </summary>
    public IReadOnlyDictionary<Guid, IMaterialInterface> MaterialInterfaces { get; }

    /// <summary>
    /// Lokales Koordinatensystem des Partikels. Bearbeitung über <see cref="CenterCoordinates"/> und <see cref="RotationAngle"/>. Sollte nicht direkt verändert werden!!!
    /// </summary>
    internal PolarCoordinateSystem LocalCoordinateSystem { get; }

    /// <summary>
    /// Koordinaten des Ursprungs des lokalen Koordinatensystem ausgedrückt im Koordinatensystem des <see cref="Parent"/>
    /// </summary>
    public AbsolutePoint CenterCoordinates { get; }

    /// <summary>
    /// Drehwinkel des Partikels.
    /// </summary>
    public Angle RotationAngle { get; }

    public IReadOnlyNodeCollection<NodeBase> Nodes => _nodes;

    IReadOnlyNodeCollection<INode> IParticle.Nodes => Nodes;

    /// <summary>
    /// Reference to the current solver session.
    /// </summary>
    private ISolverSession SolverSession { get; }

    public double MeanRadius => _meanRadius ??= Sqrt(Nodes.Sum(n => n.Volume.ToUpper)) / PI;

    public Particle ApplyTimeStep(Particle? parent, StepVector stepVector, double timeStepWidth)
    {
        Particle particle;

        if (parent is null) // is root particle
        {
            particle = new Particle(Id, CenterCoordinates, RotationAngle, Material, MaterialInterfaces, SolverSession);
        }
        else
        {
            var contactView = stepVector[parent, this];

            var displacementVector = new PolarVector(
                contactView.AngleDisplacement * timeStepWidth,
                contactView.RadialDisplacement * timeStepWidth,
                parent.LocalCoordinateSystem
            );

            var rotationAngle = (RotationAngle + contactView.RotationDisplacement * timeStepWidth).Reduce();

            particle = new Particle(Id, CenterCoordinates + displacementVector.Absolute, rotationAngle, Material, MaterialInterfaces, SolverSession);
        }

        particle._nodes = Nodes.Select(n => n.ApplyTimeStep(stepVector, timeStepWidth, particle)).ToReadOnlyNodeCollection();
        return particle;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{GetType().Name} {Id}";

    /// <inheritdoc />
    public virtual bool Equals(IVertex? other) => other is IParticle && Id == other.Id;

    /// <summary>
    /// Bestimmt die beiden einem Winkel nächstgelegenen Oberflächenknoten.
    /// </summary>
    /// <param name="angle">Winkel</param>
    /// <returns></returns>
    public (NodeBase Upper, NodeBase Lower) GetNearestNodesToAngle(Angle angle)
    {
        var nodes = Nodes.OrderBy(k => Angle.ReduceRadians(k.Coordinates.Phi.Radians, Angle.ReductionDomain.AllPositive)).ToArray();
        var upper = nodes.FirstOrDefault(k => k.Coordinates.Phi.Radians > angle.Radians) ?? nodes.First();
        var lower = upper.Lower;
        return (upper, lower);
    }

    /// <summary>
    /// Berechnet den zwischen den angrenzenden Knoten interpolierten Radius an einer bestimmten Winkelkoordinate.
    /// </summary>
    /// <param name="angle">Winkel</param>
    /// <returns></returns>
    public double InterpolatedRadius(Angle angle)
    {
        var (upper, lower) = GetNearestNodesToAngle(angle);
        return lower.Coordinates.R + (upper.Coordinates.R - lower.Coordinates.R) /
            (upper.Coordinates.Phi - lower.Coordinates.Phi).Reduce().Radians * (angle - lower.Coordinates.Phi).Reduce().Radians;
    }
}