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
public class Particle : IParticle, ITreeItem<Particle>
{
    public Particle(
        Particle? parent,
        IParticle particle,
        ISolverSession solverSession
    )
    {
        Id = particle.Id;
        Parent = parent;

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

        Surface = new ParticleSurface(this, particle.Nodes, solverSession);
        Children = new TreeChildrenCollection<Particle>(this);
        SolverSession = solverSession;
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
    public AbsolutePoint CenterCoordinates { get; private set; }

    /// <summary>
    /// Drehwinkel des Partikels.
    /// </summary>
    public Angle RotationAngle { get; private set; }

    /// <summary>
    /// Ring of surface nodes.
    /// </summary>
    public ParticleSurface Surface { get; }

    /// <inheritdoc />
    public IReadOnlyList<INode> Nodes => Surface.ToArray();

    /// <inheritdoc cref="IParticle.this[int]"/>
    public INode this[int i] => i >= 0 ? Nodes[(i % Nodes.Count)] : Nodes[^-(i % Nodes.Count)];

    INode IParticle.this[int i] => this[i];

    /// <inheritdoc cref="IParticle.this[Guid]"/>
    public INode this[Guid nodeId] => Nodes.FirstOrDefault(n => n.Id == nodeId) ??
                                      throw new IndexOutOfRangeException($"A node with ID {nodeId} is not present in this particle.");

    INode IParticle.this[Guid nodeId] => this[nodeId];

    /// <summary>
    /// Übergeordnetes Partikel dieses Partikels in der Baumanordnung.
    /// </summary>
    public Particle? Parent { get; set; }

    /// <summary>
    /// Untergeordnete Partikel dieses Partikels in der Baumanordnung.
    /// </summary>
    public TreeChildrenCollection<Particle> Children { get; }

    /// <summary>
    /// Reference to the current solver session.
    /// </summary>
    private ISolverSession SolverSession { get; }

    public virtual void ApplyTimeStep(StepVector stepVector, double timeStepWidth)
    {
        var particleView = stepVector[this];

        var displacementVector = new PolarVector(
            particleView.AngleDisplacement * timeStepWidth,
            particleView.RadialDisplacement * timeStepWidth,
            LocalCoordinateSystem
        );

        CenterCoordinates += displacementVector.Absolute;

        RotationAngle = (RotationAngle + particleView.RotationDisplacement * timeStepWidth).Reduce();

        foreach (var node in Surface)
        {
            node.ApplyTimeStep(stepVector, timeStepWidth);
        }
    }

    public virtual void ApplyState(IParticle state)
    {
        CheckState(state);

        CenterCoordinates = state.CenterCoordinates;
        RotationAngle = state.RotationAngle;

        var nodeStates = state.Nodes.ToDictionary(n => n.Id);

        foreach (var node in Surface)
        {
            node.ApplyState(nodeStates[node.Id]);
        }
    }

    protected virtual void CheckState(IParticle state)
    {
        if (state.Id != Id)
            throw new InvalidOperationException("IDs of node and state do not match.");
    }

    /// <inheritdoc/>
    public override string ToString() => $"{GetType().Name} {Id}";

    /// <inheritdoc />
    public virtual bool Equals(IVertex other) => other is IParticle && Id == other.Id;
}