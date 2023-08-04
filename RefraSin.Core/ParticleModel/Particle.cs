using System;
using System.Collections.Generic;
using System.Linq;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel.Interfaces;
using RefraSin.Core.ParticleModel.TimeSteps;
using RefraSin.Enumerables;

namespace RefraSin.Core.ParticleModel;

/// <summary>
/// Stellt ein Pulverpartikel dar.
/// </summary>
public class Particle : IParticle, ITreeItem<Particle>
{
    private Particle(
        (Angle phi, double r) centerCoordinates,
        Angle rotationAngle,
        Guid id,
        Material material,
        IReadOnlyDictionary<Material, MaterialInterface> materialInterfaces
    )
    {
        Id = id;

        CenterCoordinates = new PolarPoint(centerCoordinates)
        {
            SystemSource = () => Parent?.LocalCoordinateSystem ?? PolarCoordinateSystem.Default
        };

        RotationAngle = rotationAngle;

        LocalCoordinateSystem = new PolarCoordinateSystem
        {
            OriginSource = () => CenterCoordinates,
            RotationAngleSource = () => RotationAngle
        };

        Material = material;
        MaterialInterfaces = materialInterfaces;

        Surface = new ParticleSurface(this);
        Children = new TreeChildrenCollection<Particle>(this);
    }

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <summary>
    /// Ring of surface nodes.
    /// </summary>
    public ParticleSurface Surface { get; }

    /// <inheritdoc/>
    public Material Material { get; }

    public IReadOnlyDictionary<Material, MaterialInterface> MaterialInterfaces { get; }

    /// <summary>
    /// Lokales Koordinatensystem des Partikels. Bearbeitung über <see cref="CenterCoordinates"/> und <see cref="RotationAngle"/>. Sollte nicht direkt verändert werden!!!
    /// </summary>
    internal PolarCoordinateSystem LocalCoordinateSystem { get; }

    /// <summary>
    /// ID
    /// </summary>

    public IReadOnlyList<INeck> Necks
    {
        get
        {
            var necks = new List<Neck>();
            foreach (var node in Surface)
            {
                if (node is NeckNode neckNode)
                    if (node.Upper is GrainBoundaryNode)
                        necks.Add(new Neck(neckNode));
            }

            return necks;
        }
    }

    /// <summary>
    /// Drehwinkel des Partikels.
    /// </summary>
    public Angle RotationAngle { get; private set; }

    /// <inheritdoc />
    public AbsolutePoint AbsoluteCenterCoordinates => CenterCoordinates.Absolute;

    /// <inheritdoc />
    public IReadOnlyList<INode> SurfaceNodes => Surface.ToArray();

    /// <summary>
    /// Koordinaten des Ursprungs des lokalen Koordinatensystem ausgedrückt im Koordinatensystem des <see cref="Parent"/>
    /// </summary>
    public PolarPoint CenterCoordinates { get; private set; }

    /// <summary>
    /// Übergeordnetes Partikel dieses Partikels in der Baumanordnung.
    /// </summary>
    public Particle? Parent { get; set; }

    /// <summary>
    /// Untergeordnete Partikel dieses Partikels in der Baumanordnung.
    /// </summary>
    public TreeChildrenCollection<Particle> Children { get; }

    public virtual void ApplyTimeStep(IParticleTimeStep timeStep)
    {
        CheckTimeStep(timeStep);

        CenterCoordinates += timeStep.DisplacementVector;

        RotationAngle = (RotationAngle + timeStep.RotationDisplacement).Reduce();

        foreach (var node in Surface)
        {
            node.ApplyTimeStep(timeStep.NodeTimeSteps[node.Id]);
        }
    }

    private void CheckTimeStep(IParticleTimeStep timeStep)
    {
        if (timeStep.ParticleId != Id)
            throw new InvalidOperationException("IDs of particle and time step do not match.");

        if (timeStep.DisplacementVector.System != CenterCoordinates.System)
            throw new InvalidOperationException("Current coordinates and displacement vector must be in same coordinate system.");
    }

    public virtual void ApplyState(IParticle state)
    {
        CheckState(state);

        CenterCoordinates = state.CenterCoordinates;
        RotationAngle = state.RotationAngle;

        var nodeStates = state.SurfaceNodes.ToDictionary(n => n.Id);

        foreach (var node in Surface)
        {
            node.ApplyState(nodeStates[node.Id]);
        }
    }

    protected virtual void CheckState(IParticle state)
    {
        if (state.Id != Id)
            throw new InvalidOperationException("IDs of node and state do not match.");

        if (state.CenterCoordinates.System != CenterCoordinates.System)
            throw new InvalidOperationException("Current coordinates and state coordinates must be in same coordinate system.");
    }

    /// <inheritdoc/>
    public override string ToString() => $"{GetType().Name} {Id.ToShortString()}";
}