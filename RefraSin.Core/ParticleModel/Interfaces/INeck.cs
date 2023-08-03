using System;
using System.Collections.Generic;

namespace RefraSin.Core.ParticleModel.Interfaces;

/// <summary>
/// Interface representing a sinter neck between two particles from the viewpoint of one of them.
/// </summary>
public interface INeck
{
    /// <summary>
    /// Lower end neck node.
    /// </summary>
    public INeckNode LowerNeckNode { get; }

    /// <summary>
    /// Upper end neck node.
    /// </summary>
    public INeckNode UpperNeckNode { get; }

    /// <summary>
    /// List of all nodes the grain boundary consists of.
    /// </summary>
    public IReadOnlyList<IGrainBoundaryNode> GrainBoundaryNodes { get; }

    /// <summary>
    /// ID of the particle the neck is viewed from.
    /// </summary>
    public Guid ParticleId { get; }

    /// <summary>
    /// ID of the other participating particle.
    /// </summary>
    public Guid ContactedParticleId { get; }

    /// <summary>
    /// Average radius of the neck (projected grain boundary length).
    /// </summary>
    public double Radius { get; }

    /// <summary>
    /// Total length of the grain boundary.
    /// </summary>
    public double GrainBoundaryLength { get; }

    /// <summary>
    /// Unique ID of the neck.
    /// </summary>
    public int Id { get; }
}