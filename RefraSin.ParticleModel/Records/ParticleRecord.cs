using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;

namespace RefraSin.ParticleModel.Records;

/// <summary>
/// Represents an immutable record of a particle.
/// </summary>
public record ParticleRecord(
    Guid Id,
    Angle RotationAngle,
    PolarPoint CenterCoordinates,
    AbsolutePoint AbsoluteCenterCoordinates,
    IReadOnlyList<INode> SurfaceNodes,
    IReadOnlyList<INeck> Necks
) : IParticle
{
    /// <summary>
    /// Kopierkonstruktor.
    /// </summary>
    /// <param name="template">Vorlage</param>
    public ParticleRecord(IParticle template) : this(
        template.Id,
        template.RotationAngle,
        template.CenterCoordinates,
        template.AbsoluteCenterCoordinates,
        template.SurfaceNodes.Select<INode, INode>(
            k => k switch
            {
                INeckNode nk          => new NeckNodeRecord(nk),
                IGrainBoundaryNode ck => new GrainBoundaryNodeRecord(ck),
                _                     => new SurfaceNodeRecord(k)
            }
        ).ToArray(),
        template.Necks.Select(n => new NeckRecord(n)).ToArray()
    ) { }

    /// <inheritdoc />
    public Guid Id { get; } = Id;

    /// <inheritdoc />
    public Angle RotationAngle { get; } = RotationAngle;

    /// <inheritdoc />
    public PolarPoint CenterCoordinates { get; } = CenterCoordinates;

    /// <inheritdoc />
    public AbsolutePoint AbsoluteCenterCoordinates { get; } = AbsoluteCenterCoordinates;

    /// <inheritdoc />
    public IReadOnlyList<INode> SurfaceNodes { get; } = SurfaceNodes;

    /// <inheritdoc />
    public IReadOnlyList<INeck> Necks { get; } = Necks;
}