using System;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;
using RefraSin.Core.ParticleModel.HelperTypes;
using RefraSin.Core.ParticleModel.Interfaces;
using RefraSin.Core.ParticleModel.TimeSteps;
using RefraSin.Core.Solver;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Oberflächenknoten, der Teil einer freien Oberfläche ist.
    /// </summary>
    public class SurfaceNode : Node, ISurfaceNode
    {
        /// <inheritdoc />
        public SurfaceNode((Angle phi, double r) coordinates) : base(coordinates) { }

        /// <inheritdoc />
        public SurfaceNode((Angle phi, double r) coordinates, Guid id) : base(coordinates, id) { }

        /// <inheritdoc />
        public override ToUpperToLower SurfaceEnergy => _surfaceEnergy ??= new ToUpperToLower(
            Particle.Material.SurfaceEnergy,
            Particle.Material.SurfaceEnergy
        );

        private ToUpperToLower? _surfaceEnergy;

        /// <inheritdoc />
        public override ToUpperToLower SurfaceDiffusionCoefficient => _surfaceDiffusionCoefficient ??= new ToUpperToLower(
            Particle.Material.SurfaceDiffusionCoefficient,
            Particle.Material.SurfaceDiffusionCoefficient
        );

        private ToUpperToLower? _surfaceDiffusionCoefficient;

        /// <inheritdoc />
        public override Node ApplyTimeStep(INodeTimeStep timeStep)
        {
            CheckTimeStep(timeStep);

            var newCoordinates = Coordinates + timeStep.DisplacementVector;
            var newNode = new SurfaceNode(newCoordinates.ToTuple(), Id);

            return newNode;
        }
    }
}