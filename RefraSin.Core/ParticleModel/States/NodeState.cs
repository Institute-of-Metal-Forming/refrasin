using System;
using System.Globalization;
using IMF.Coordinates.Absolute;
using IMF.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;
using RefraSin.Core.ParticleModel.Interfaces;

namespace RefraSin.Core.ParticleModel.States
{
    /// <summary>
    /// Stellt den Zustand eines Oberflächenknotens dar.
    /// </summary>
    public abstract class NodeState : INode
    {
        /// <summary>
        /// Kopierkonstruktor.
        /// </summary>
        /// <param name="template">Vorlage</param>
        public NodeState(INode template)
        {
            Id = template.Id;
            ParticleId = template.ParticleId;
            Coordinates = template.Coordinates;
            AbsoluteCoordinates = template.AbsoluteCoordinates;
            Curvature = template.Curvature;
            SurfaceTension = template.SurfaceTension;
            DeviatoricChemicalPotential = template.DeviatoricChemicalPotential;
            DeviatoricVacancyConcentration = template.DeviatoricVacancyConcentration;
            DiffusionalFlowBalance = template.DiffusionalFlowBalance;
            SurfaceDistance = template.SurfaceDistance;
            VacancyConcentration = template.VacancyConcentration;
        }
        
        /// <inheritdoc />
        public Guid Id { get; set; }

        /// <inheritdoc />
        public Guid ParticleId { get; set; }

        /// <inheritdoc />
        public PolarPoint Coordinates { get; set; }

        /// <inheritdoc />
        public AbsolutePoint AbsoluteCoordinates { get; set; }

        /// <inheritdoc />
        public double Curvature { get; set; }

        /// <inheritdoc />
        public double SurfaceTension { get; set; }

        /// <inheritdoc />
        public double DeviatoricChemicalPotential { get; set; }

        /// <inheritdoc />
        public double DeviatoricVacancyConcentration { get; set; }

        /// <inheritdoc />
        public double VacancyConcentration { get; }

        /// <inheritdoc />
        public double DiffusionalFlowBalance { get; set; }

        /// <inheritdoc />
        public ToUpperToLower SurfaceDistance { get; set; }
        
        /// <inheritdoc />
        public override string ToString() => $"{GetType()} {Id.ToShortString()} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)}";
    }
}