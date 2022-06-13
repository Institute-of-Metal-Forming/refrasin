using System;
using System.Collections.Generic;
using IMF.Coordinates;
using IMF.Coordinates.Absolute;
using IMF.Coordinates.Polar;
using RefraSin.Core.Materials;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Schnittstelle für Pulverteilchen.
    /// </summary>
    public interface IParticle
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Drehwinkel des Partikels.
        /// </summary>
        public Angle RotationAngle { get; }

        /// <summary>
        /// Koordinaten der Teilchenmitte.
        /// </summary>
        public AbsolutePoint AbsoluteCenterCoordinates { get; }

        /// <summary>
        /// Koordinaten der Teilchenmitte.
        /// </summary>
        public PolarPoint CenterCoordinates { get; }
        
        /// <summary>
        /// Materialdaten.
        /// </summary>
        public Material Material { get; }

        /// <summary>
        /// Liste der Oberflächenknoten.
        /// </summary>
        public IReadOnlyList<INode> SurfaceNodes { get; }

        public IReadOnlyList<INeck> Necks { get; }
    }
}