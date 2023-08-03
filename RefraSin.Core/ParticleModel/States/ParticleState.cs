using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using IMF.Coordinates;
using IMF.Coordinates.Absolute;
using IMF.Coordinates.Polar;
using IMF.Enumerables;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel.Interfaces;

namespace RefraSin.Core.ParticleModel.States
{
    /// <summary>
    /// Stellt einen Zustand eines Partikels dar.
    /// </summary>
    public class ParticleState : IParticle
    {
        /// <summary>
        /// Kopierkonstruktor.
        /// </summary>
        /// <param name="template">Vorlage</param>
        public ParticleState(IParticle template)
        {
            Id = template.Id;
            RotationAngle = template.RotationAngle;
            CenterCoordinates = template.CenterCoordinates.Clone();
            AbsoluteCenterCoordinates = template.CenterCoordinates.Absolute;
            SurfaceNodes = new List<INode>(
                template.SurfaceNodes.Select<INode, INode>(
                    k => k switch
                    {
                        INeckNode nk          => new NeckNodeState(nk),
                        IGrainBoundaryNode ck => new GrainBoundaryNodeState(ck),
                        _                     => new SurfaceNodeState(k)
                    }
                )
            );
            Necks = template.Necks.Select(n => new NeckState(n)).ToArray();
            Material = template.Material;
        }

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public Angle RotationAngle { get; }

        /// <inheritdoc />
        public PolarPoint CenterCoordinates { get; }

        /// <inheritdoc />
        public Material Material { get; }

        /// <inheritdoc />
        public AbsolutePoint AbsoluteCenterCoordinates { get; }

        /// <inheritdoc />
        public IReadOnlyList<INode> SurfaceNodes { get; }

        public IReadOnlyList<INeck> Necks { get; }

        /// <inheritdoc />
        public override string ToString() => $"ParticleState {Id.ToShortString()}";
    }
}