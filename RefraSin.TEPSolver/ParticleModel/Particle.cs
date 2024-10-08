using System.Globalization;
using RefraSin.Coordinates;
using RefraSin.Graphs;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Collections;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Stellt ein Pulverpartikel dar.
/// </summary>
public class Particle : IParticle<NodeBase>, IParticleContacts<Particle>
{
    private ReadOnlyParticleSurface<NodeBase> _nodes;
    private IReadOnlyContactCollection<IParticleContactEdge<Particle>>? _contacts;

    public Particle(IParticle<IParticleNode> particle, SolutionState solutionState, ISinteringConditions conditions)
    {
        Id = particle.Id;
        Coordinates = particle.Coordinates.Absolute;
        RotationAngle = particle.RotationAngle;

        SolutionState = solutionState;

        MaterialId = particle.MaterialId;
        var material = solutionState.Materials[particle.MaterialId];
        VacancyVolumeEnergy =
            conditions.Temperature
            * conditions.GasConstant
            / (material.Substance.MolarVolume * material.Bulk.EquilibriumVacancyConcentration);

        SurfaceProperties = material.Surface;
        InterfaceProperties = material.Interfaces;

        _nodes = particle
            .Nodes.Select(node =>
                node switch
                {
                    { Type: NodeType.GrainBoundary }
                        => new GrainBoundaryNode(node, this),
                    { Type: NodeType.Neck } => new NeckNode(node, this),
                    _ => (NodeBase)new SurfaceNode(node, this),
                }
            )
            .ToReadOnlyParticleSurface();
    }

    private Particle(
        Particle? parent,
        Particle previousState,
        StepVector stepVector,
        double timeStepWidth
    )
    {
        Id = previousState.Id;

        MaterialId = previousState.MaterialId;
        VacancyVolumeEnergy = previousState.VacancyVolumeEnergy;
        SurfaceProperties = previousState.SurfaceProperties;
        InterfaceProperties = previousState.InterfaceProperties;

        SolutionState = previousState.SolutionState;

        // Apply time step changes
        if (parent is null) // is root particle
        {
            Coordinates = previousState.Coordinates;
            RotationAngle = previousState.RotationAngle;
        }
        else
        {
            var contact = SolutionState.ParticleContacts[parent.Id, previousState.Id];
            var polarCoordinates = contact.ContactVector;
            var newCoordinates = new PolarPoint(
                polarCoordinates.Phi + stepVector.AngleDisplacement(contact) * timeStepWidth,
                polarCoordinates.R + stepVector.RadialDisplacement(contact) * timeStepWidth,
                parent
            );
            Coordinates = newCoordinates.Absolute;

            RotationAngle = previousState.RotationAngle + stepVector.RotationDisplacement(contact) * timeStepWidth;
        }

        _nodes = previousState
            .Nodes.Select(n => n.ApplyTimeStep(stepVector, timeStepWidth, this))
            .ToReadOnlyParticleSurface();
    }

    public SolutionState SolutionState { get; }

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <inheritdoc />
    public Guid MaterialId { get; }

    public IInterfaceProperties SurfaceProperties { get; }

    /// <summary>
    /// Dictionary of material IDs to material interface data, assuming that the current instances material is always on the from side.
    /// </summary>
    public IReadOnlyDictionary<Guid, IInterfaceProperties> InterfaceProperties { get; }

    /// <summary>
    /// Koordinaten des Ursprungs des lokalen Koordinatensystem ausgedrückt im Koordinatensystem des <see cref="Parent"/>
    /// </summary>
    public AbsolutePoint Coordinates { get; }

    ICartesianPoint IParticle.Coordinates => Coordinates;

    /// <summary>
    /// Drehwinkel des Partikels.
    /// </summary>
    public Angle RotationAngle { get; }

    public IReadOnlyParticleSurface<NodeBase> Nodes => _nodes;

    public double VacancyVolumeEnergy { get; }

    public Particle ApplyTimeStep(Particle? parent, StepVector stepVector, double timeStepWidth) =>
        new(parent, this, stepVector, timeStepWidth);

    /// <inheritdoc/>
    public override string ToString() =>
        $"{GetType().Name} {Id} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)}";

    /// <inheritdoc />
    public virtual bool Equals(IVertex? other) => other is IParticle && Id == other.Id;

    /// <inheritdoc />
    public IReadOnlyContactCollection<IParticleContactEdge<Particle>> Contacts => _contacts ??=  SolutionState.ParticleContacts.From(Id).ToReadOnlyContactCollection();
}
