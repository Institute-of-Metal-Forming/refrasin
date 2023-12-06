using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Enumerables;
using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Stellt ein Pulverpartikel dar.
/// </summary>
public class Particle : IParticle
{
    private NodeBase[] _nodesArray;
    private Dictionary<Guid, int> _nodeIndices = null!;

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
        _nodesArray = particle.Nodes.Select(node => (NodeBase)node switch
        {
            INeckNode neckNode                   => new NeckNode(neckNode, this, solverSession),
            IGrainBoundaryNode grainBoundaryNode => new GrainBoundaryNode(grainBoundaryNode, this, solverSession),
            _                                    => (NodeBase)new SurfaceNode(node, this, solverSession),
        }).ToArray();
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
        _nodesArray = Array.Empty<NodeBase>();
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

    public IReadOnlyList<NodeBase> Nodes => _nodesArray;

    private NodeBase[] NodesArray
    {
        get => _nodesArray;
        set
        {
            _nodesArray = value;
            _nodeIndices = value.Select((n, i) => (n.Id, i)).ToDictionary(t => t.Id, t => t.i);
        }
    }

    IReadOnlyList<INode> IParticle.Nodes => Nodes;

    public int GetNodeIndex(NodeBase node) => _nodeIndices[node.Id];

    /// <inheritdoc cref="IParticle.this[int]"/>
    public NodeBase this[int i] => i >= 0 ? Nodes[(i % Nodes.Count)] : Nodes[^-(i % Nodes.Count)];

    INode IParticle.this[int i] => this[i];

    /// <inheritdoc cref="IParticle.this[Guid]"/>
    public NodeBase this[Guid nodeId] => Nodes.FirstOrDefault(n => n.Id == nodeId) ??
                                         throw new IndexOutOfRangeException($"A node with ID {nodeId} is not present in this particle.");

    INode IParticle.this[Guid nodeId] => this[nodeId];

    /// <summary>
    /// Reference to the current solver session.
    /// </summary>
    private ISolverSession SolverSession { get; }

    public Particle ApplyTimeStep(StepVector stepVector, double timeStepWidth)
    {
        var particleView = stepVector[this];

        var displacementVector = new PolarVector(
            particleView.AngleDisplacement * timeStepWidth,
            particleView.RadialDisplacement * timeStepWidth,
            LocalCoordinateSystem
        );

        var rotationAngle = (RotationAngle + particleView.RotationDisplacement * timeStepWidth).Reduce();

        var particle = new Particle(Id, CenterCoordinates + displacementVector.Absolute, rotationAngle, Material, MaterialInterfaces, SolverSession);

        particle.NodesArray = Nodes.Select(n => n.ApplyTimeStep(stepVector, timeStepWidth, particle)).ToArray();
        return particle;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{GetType().Name} {Id}";

    /// <inheritdoc />
    public virtual bool Equals(IVertex other) => other is IParticle && Id == other.Id;

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