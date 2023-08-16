using System.Globalization;
using Microsoft.Extensions.Logging;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;
using RefraSin.Coordinates.Polar;
using RefraSin.Enumerables;
using RefraSin.ParticleModel;
using static System.Math;

namespace RefraSin.TEPSolver.ParticleModel
{
    /// <summary>
    /// Abstract base class for particle surface nodes.
    /// </summary>
    public abstract class Node : INode, IRingItem<Node>
    {
        protected Node((Angle phi, double r) coordinates, Guid id)
        {
            Id = id;
            Coordinates = new PolarPoint(coordinates) { SystemSource = () => Particle.LocalCoordinateSystem };
        }

        protected Node((Angle phi, double r) coordinates) : this(coordinates, Guid.NewGuid()) { }

        public Guid Id { get; set; }

        private Node? _lower;
        private Node? _upper;

        /// <summary>
        ///     Partikel, zu dem dieser Knoten gehört.
        /// </summary>
        public Particle Particle => Surface.Particle;

        /// <inheritdoc />
        public Guid ParticleId => Particle.Id;

        /// <summary>
        /// A reference to the upper neighbor of this node.
        /// </summary>
        /// <exception cref="InvalidNeighborhoodException">If this node has currently no upper neighbor set.</exception>
        public Node Upper => _upper ?? throw new InvalidNeighborhoodException(this, InvalidNeighborhoodException.Neighbor.Upper);

        /// <summary>
        /// A reference to the lower neighbor of this node.
        /// </summary>
        /// <exception cref="InvalidNeighborhoodException">If this node has currently no lower neighbor set.</exception>
        public Node Lower => _lower ?? throw new InvalidNeighborhoodException(this, InvalidNeighborhoodException.Neighbor.Lower);

        /// <inheritdoc />
        Node? IRingItem<Node>.Upper
        {
            get => _upper;
            set => _upper = value;
        }

        /// <inheritdoc />
        Node? IRingItem<Node>.Lower
        {
            get => _lower;
            set => _lower = value;
        }

        /// <inheritdoc />
        Ring<Node>? IRingItem<Node>.Ring { get; set; }

        /// <summary>
        /// A reference to the <see cref="ParticleSurface"/> instance this node is part of.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the node is not included in a surface.</exception>
        private ParticleSurface Surface => (ParticleSurface?)((IRingItem<Node>)this).Ring ??
                                           throw new InvalidOperationException($"The node {this} is not included in a particle surface.");

        /// <summary>
        /// Coordinates of the node in terms of particle's local coordinate system <see cref="ParticleModel.Particle.LocalCoordinateSystem" />
        /// </summary>
        public PolarPoint Coordinates { get; private set; }

        /// <inheritdoc />
        public AbsolutePoint AbsoluteCoordinates => Coordinates.Absolute;

        /// <summary>
        ///     Winkeldistanz zu den Nachbarknoten (Größe des kürzesten Winkels).
        /// </summary>
        public ToUpperToLowerAngle AngleDistance => _angleDistance ??= new ToUpperToLowerAngle(
            Coordinates.AngleTo(Upper.Coordinates),
            Coordinates.AngleTo(Lower.Coordinates)
        );

        private ToUpperToLowerAngle? _angleDistance;

        /// <summary>
        ///     Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
        /// </summary>
        public ToUpperToLower SurfaceDistance => _surfaceDistance ??= new ToUpperToLower(
            CosLaw.C(Upper.Coordinates.R, Coordinates.R, AngleDistance.ToUpper),
            CosLaw.C(Lower.Coordinates.R, Coordinates.R, AngleDistance.ToLower)
        );

        private ToUpperToLower? _surfaceDistance;

        /// <summary>
        ///     Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
        /// </summary>
        public ToUpperToLowerAngle SurfaceRadiusAngle => _surfaceRadiusAngle ??= new ToUpperToLowerAngle(
            CosLaw.Gamma(SurfaceDistance.ToUpper, Coordinates.R, Upper.Coordinates.R),
            CosLaw.Gamma(SurfaceDistance.ToLower, Coordinates.R, Lower.Coordinates.R)
        );

        private ToUpperToLowerAngle? _surfaceRadiusAngle;

        /// <summary>
        ///     Gesamtes Volumen der an den Knoten angrenzenden Elemente.
        /// </summary>
        public ToUpperToLower Volume => _volume ??= new ToUpperToLower(
            0.5 * Coordinates.R * Upper.Coordinates.R * Sin(AngleDistance.ToUpper),
            0.5 * Coordinates.R * Lower.Coordinates.R * Sin(AngleDistance.ToLower)
        );

        private ToUpperToLower? _volume;

        public NormalTangentialAngle SurfaceAngle => _surfaceAngle ??= new NormalTangentialAngle(
            PI - 0.5 * SurfaceRadiusAngle.Sum,
            PI / 2 - 0.5 * SurfaceRadiusAngle.Sum
        );

        private NormalTangentialAngle? _surfaceAngle;

        /// <inheritdoc />
        public abstract ToUpperToLower SurfaceEnergy { get; }

        /// <inheritdoc />
        public abstract ToUpperToLower SurfaceDiffusionCoefficient { get; }

        /// <inheritdoc />
        public NormalTangential GibbsEnergyGradient => _gibbsEnergyGradient ??= new NormalTangential(
            -(SurfaceEnergy.ToUpper + SurfaceEnergy.ToLower) * Cos(SurfaceAngle.Normal),
            -(SurfaceEnergy.ToUpper - SurfaceEnergy.ToLower) * Cos(SurfaceAngle.Tangential)
        );

        private NormalTangential? _gibbsEnergyGradient;

        /// <inheritdoc />
        public NormalTangential VolumeGradient => _volumeGradient ??= new NormalTangential(
            0.5 * (SurfaceDistance.ToUpper + SurfaceDistance.ToLower) * Sin(SurfaceAngle.Normal),
            0.5 * (SurfaceDistance.ToUpper - SurfaceDistance.ToLower) * Sin(SurfaceAngle.Tangential)
        );

        private NormalTangential? _volumeGradient;

        protected virtual void ClearCaches()
        {
            _angleDistance = null;
            _surfaceDistance = null;
            _surfaceRadiusAngle = null;
            _volume = null;
            _surfaceAngle = null;
            _gibbsEnergyGradient = null;
            _volumeGradient = null;
        }

        public virtual void ApplyTimeStep(INodeTimeStep timeStep)
        {
            CheckTimeStep(timeStep);

            Coordinates += timeStep.DisplacementVector;

            ClearCaches();
        }

        protected virtual void CheckTimeStep(INodeTimeStep timeStep)
        {
            if (timeStep.NodeId != Id)
                throw new InvalidOperationException("IDs of node and time step do not match.");

            if (timeStep.DisplacementVector.System != Coordinates.System)
                throw new InvalidOperationException("Current coordinates and displacement vector must be in same coordinate system.");
        }

        public virtual void ApplyState(INode state)
        {
            CheckState(state);

            Coordinates = new PolarPoint(state.Coordinates.ToTuple()) { SystemSource = () => Particle.LocalCoordinateSystem };

            _angleDistance = state.AngleDistance;
            _surfaceDistance = state.SurfaceDistance;
            _surfaceRadiusAngle = state.SurfaceRadiusAngle;
            _volume = state.Volume;
            _surfaceAngle = state.SurfaceAngle;
            _gibbsEnergyGradient = state.GibbsEnergyGradient;
            _volumeGradient = state.VolumeGradient;

            ClearCaches();
        }


        protected virtual void CheckState(INode state)
        {
            if (state.Id != Id)
                throw new InvalidOperationException("IDs of node and state do not match.");

            if (state.Coordinates.System != Coordinates.System)
                throw new InvalidOperationException("Current coordinates and state coordinates must be in same coordinate system.");
        }

        /// <summary>
        ///     Gibt die Repräsentation des Knotens als String zurück.
        ///     Format: "{Typname} {Id} of {Partikel}".
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"{GetType().Name} {Id} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)} of {Particle}";

        /// <summary>
        ///     Gibt die Repräsentation des Knotens als String zurück.
        ///     Format: "{Typname} {Id} of {Partikel}".
        /// </summary>
        /// <returns></returns>
        public string ToString(bool shortVersion)
        {
            if (shortVersion)
                return
                    $"{GetType().Name} {Id}";
            return ToString();
        }
    }
}