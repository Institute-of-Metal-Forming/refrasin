using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;

namespace RefraSin.ParticleModel;

/// <summary>
/// Represents an immutable record of a particle.
/// </summary>
public record Particle : IParticle
{
    /// <summary>
    /// Represents an immutable record of a particle.
    /// </summary>
    public Particle(
        Guid id,
        PolarPoint centerCoordinates,
        AbsolutePoint absoluteCenterCoordinates,
        Angle rotationAngle,
        Guid materialId,
        IReadOnlyList<INode> nodes,
        IReadOnlyList<INeck>? necks = null
    )
    {
        Id = id;
        CenterCoordinates = new PolarPoint(centerCoordinates, centerCoordinates.System);
        AbsoluteCenterCoordinates = new AbsolutePoint(absoluteCenterCoordinates.ToTuple());
        RotationAngle = rotationAngle;
        MaterialId = materialId;
        Nodes = nodes;
        Necks = necks ?? Array.Empty<INeck>();
    }

    /// <summary>
    /// Kopierkonstruktor.
    /// </summary>
    /// <param name="template">Vorlage</param>
    public Particle(IParticle template) : this(
        template.Id,
        template.CenterCoordinates,
        template.AbsoluteCenterCoordinates,
        template.RotationAngle,
        template.MaterialId,
        template.Nodes.Select<INode, INode>(
            k => k switch
            {
                INeckNode nk          => new NeckNode(nk),
                IGrainBoundaryNode ck => new GrainBoundaryNode(ck),
                _                     => new SurfaceNode(k)
            }
        ).ToArray(),
        template.Necks.Select(n => new Neck(n)).ToArray()
    ) { }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public PolarPoint CenterCoordinates { get; }

    /// <inheritdoc />
    public AbsolutePoint AbsoluteCenterCoordinates { get; }

    /// <inheritdoc />
    public Angle RotationAngle { get; }

    /// <inheritdoc />
    public IReadOnlyList<INode> Nodes { get; }

    IReadOnlyList<INodeSpec> IParticleSpec.NodeSpecs => Nodes;

    /// <inheritdoc />
    public IReadOnlyList<INeck> Necks { get; }

    /// <inheritdoc />
    public Guid MaterialId { get; }

    /// <inheritdoc cref="IParticleSpec.this[int]"/>
    public INode this[int i] => i >= 0 ? Nodes[(i % Nodes.Count)] : Nodes[^-(i % Nodes.Count)];

    INodeSpec IParticleSpec.this[int i] => this[i];

    /// <inheritdoc cref="IParticleSpec.this[Guid]"/>
    public INode this[Guid nodeId] => Nodes.FirstOrDefault(n => n.Id == nodeId) ??
                                      throw new IndexOutOfRangeException($"A node with ID {nodeId} is not present in this particle.");

    INodeSpec IParticleSpec.this[Guid nodeId] => this[nodeId];

    /// <inheritdoc />
    public virtual bool Equals(IVertex other) => other is IParticleSpec && Id == other.Id;
}