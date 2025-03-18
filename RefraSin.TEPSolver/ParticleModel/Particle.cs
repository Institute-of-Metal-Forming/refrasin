using System.Globalization;
using RefraSin.Coordinates;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Collections;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Quantities;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Stellt ein Pulverpartikel dar.
/// </summary>
public class Particle : IParticle<NodeBase>
{
    private ReadOnlyParticleSurface<NodeBase> _nodes;

    public Particle(
        IParticle<IParticleNode> particle,
        SolutionState solutionState,
        ISinteringConditions conditions
    )
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
                    { Type: NodeType.GrainBoundary } => new GrainBoundaryNode(node, this),
                    { Type: NodeType.Neck } => new NeckNode(node, this),
                    _ => (NodeBase)new SurfaceNode(node, this),
                }
            )
            .ToReadOnlyParticleSurface();
    }

    private Particle(
        SolutionState solutionState,
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

        SolutionState = solutionState;

        Coordinates =
            previousState.Coordinates
            + new AbsoluteVector(
                stepVector.QuantityValue<ParticleDisplacementX>(previousState),
                stepVector.QuantityValue<ParticleDisplacementY>(previousState)
            );
        RotationAngle = previousState.RotationAngle;

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

    public Particle ApplyTimeStep(
        SolutionState solutionState,
        StepVector stepVector,
        double timeStepWidth
    ) => new(solutionState, this, stepVector, timeStepWidth);

    /// <inheritdoc/>
    public override string ToString() =>
        $"{GetType().Name} {Id} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)}";
}
