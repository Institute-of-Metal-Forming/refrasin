using System;
using System.Collections.Generic;
using System.Linq;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel.Interfaces;

namespace RefraSin.Core.ParticleModel.Records
{
    /// <summary>
    /// Represents an immutable record of a particle.
    /// </summary>
    public record ParticleRecord(
        Guid Id,
        Angle RotationAngle,
        PolarPoint CenterCoordinates,
        AbsolutePoint AbsoluteCenterCoordinates,
        Material Material,
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
            template.Material,
            template.SurfaceNodes.Select<INode, INode>(
                k => k switch
                {
                    INeckNode nk          => new NeckNodeRecord(nk),
                    IGrainBoundaryNode ck => new GrainBoundaryNodeRecord(ck),
                    _                     => new SurfaceNodeRecord(k)
                }
            ).ToArray(),
            template.Necks.Select(n => new NeckState(n)).ToArray()
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
        public Material Material { get; } = Material;

        /// <inheritdoc />
        public IReadOnlyList<INode> SurfaceNodes { get; } = SurfaceNodes;

        /// <inheritdoc />
        public IReadOnlyList<INeck> Necks { get; } = Necks;
    }
}