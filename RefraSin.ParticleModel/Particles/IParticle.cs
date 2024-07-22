using RefraSin.Coordinates;
using RefraSin.Coordinates.Cartesian;
using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParticleModel.Particles;

public interface IParticle<out TNode> : IParticle where TNode : IParticleNode
{
    /// <summary>
    /// List of node specs.
    /// </summary>
    public IReadOnlyParticleSurface<TNode> Nodes { get; }
}

public interface IParticle : IVertex, IPolarCoordinateSystem
{
    /// <summary>
    /// Absolute coordinates of the particle's center.
    /// </summary>
    public ICartesianPoint Coordinates { get; }

    /// <summary>
    /// ID of the material.
    /// </summary>
    Guid MaterialId { get; }

    IPoint ICoordinateSystem.Origin => Coordinates;

    Angle.ReductionDomain IPolarCoordinateSystem.AngleReductionDomain => Angle.ReductionDomain.AllPositive;

    double IPolarCoordinateSystem.RScale => 1;
}