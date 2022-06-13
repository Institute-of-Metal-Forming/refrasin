using System;
using IMF.Coordinates.Absolute;
using IMF.Coordinates.Polar;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Schnitstelle für Oberflächenknoten eines Partikels.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// ID des Knotens.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// ID des Partikels, zu dem dieser Knoten gehört.
        /// </summary>
        public Guid ParticleId { get; }

        /// <summary>
        /// Koordinaten des Punktes mit Basis auf <see cref="Particle.LocalCoordinateSystem"/>
        /// </summary>
        public PolarPoint Coordinates { get; }

        /// <summary>
        /// Absolute Koordinaten.
        /// </summary>
        public AbsolutePoint AbsoluteCoordinates { get; }

        /// <summary>
        /// Oberflächenkrümmung am Knoten.
        /// </summary>
        public double Curvature { get; }

        /// <summary>
        /// Oberflächenspannung durch Krümmung am Knoten. 
        /// </summary>
        public double SurfaceTension { get; }

        /// <summary>
        /// Deviatorisches chemisches Potenzial am Knoten.
        /// </summary>
        public double DeviatoricChemicalPotential { get; }

        /// <summary>
        /// Deviatorische Leerstellenkonzentration am Knoten.
        /// </summary>
        public double DeviatoricVacancyConcentration { get; }

        /// <summary>
        /// Gradient der Leerstellenkonzentration zu den Nachbarknoten.
        /// </summary>
        public double DiffusionalFlowBalance { get; }
        
        /// <summary>
        /// Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
        /// </summary>
        public ToUpperToLower SurfaceDistance { get; }
    }
}