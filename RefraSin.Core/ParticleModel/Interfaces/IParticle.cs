using System;
using System.Collections.Generic;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Core.Materials;

namespace RefraSin.Core.ParticleModel.Interfaces;

/// <summary>
/// Interface representing a powder particle.
/// </summary>
public interface IParticle
{
    /// <summary>
    /// Unique ID.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Rotation angle of the particle's coordinate system around its center.
    /// </summary>
    public Angle RotationAngle { get; }

    /// <summary>
    /// Absolute coordinates of the particle's center.
    /// </summary>
    public AbsolutePoint AbsoluteCenterCoordinates { get; }

    /// <summary>
    /// Polar coordinates of the particle's center in means of the parent's coordinate system.
    /// </summary>
    public PolarPoint CenterCoordinates { get; }
        
    /// <summary>
    /// Properties of the material, the particle consists of.
    /// </summary>
    public Material Material { get; }

    /// <summary>
    /// List of all surface nodes.
    /// </summary>
    public IReadOnlyList<INode> SurfaceNodes { get; }

    /// <summary>
    /// List of all necks to neighbor particles.
    /// </summary>
    public IReadOnlyList<INeck> Necks { get; }
}