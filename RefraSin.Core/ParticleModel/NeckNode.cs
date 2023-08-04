using System;
using Microsoft.Extensions.Logging;
using RefraSin.Coordinates;
using RefraSin.Core.ParticleModel.HelperTypes;
using RefraSin.Core.ParticleModel.Interfaces;
using RefraSin.Core.ParticleModel.TimeSteps;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Oberflächenknoten welcher am Punkt des Sinterhalse liegt. Vermittelt den Übergang zwischen freien Oberflächen und Korngrenzen.
    /// </summary>
    public class NeckNode : ContactNode<NeckNode>, INeckNode
    {
        private readonly ILogger<NeckNode> _logger = Configuration.CreateLogger<NeckNode>();

        /// <inheritdoc />
        public NeckNode((Angle phi, double r) coordinates) : base(coordinates) { }

        public override ToUpperToLower SurfaceEnergy => _surfaceEnergy ??= new ToUpperToLower(
            Upper.SurfaceEnergy.ToLower,
            Lower.SurfaceEnergy.ToUpper
        );

        private ToUpperToLower? _surfaceEnergy;

        /// <inheritdoc />
        public override ToUpperToLower SurfaceDiffusionCoefficient => _surfaceDiffusionCoefficient ??= new ToUpperToLower(
            Upper.SurfaceDiffusionCoefficient.ToLower,
            Lower.SurfaceDiffusionCoefficient.ToUpper
        );

        private ToUpperToLower? _surfaceDiffusionCoefficient;

        /// <inheritdoc />
        public override Node ApplyTimeStep(INodeTimeStep timeStep) => throw new NotImplementedException();

        /// <inheritdoc />
        public Guid OppositeNeckNodeId { get; }
    }
}