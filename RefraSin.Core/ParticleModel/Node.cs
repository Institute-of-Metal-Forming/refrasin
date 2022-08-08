using System;
using System.Globalization;
using IMF.Coordinates.Absolute;
using IMF.Coordinates.Polar;
using IMF.Enumerables;
using IMF.Maths;
using IMF.Utils;
using Microsoft.Extensions.Logging;
using RefraSin.Core.Solver;
using static System.Math;
using static MathNet.Numerics.Constants;
using static RefraSin.Core.Solver.InvalidityReason;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    ///     Abstrakte Basisklasse für Oberflächenknoten eines Partikels.
    /// </summary>
    public abstract class Node : INode, IRingItem<Node>
    {
        private PolarVector? _timeStepDisplacementVector;
        private ILogger _logger = Configuration.CreateLogger<Node>();

        private Node(Particle particle)
        {
            Id = Guid.NewGuid();
            Particle = particle;
        }

        protected Node(Particle particle, PolarPoint coordinates) : this(particle)
        {
            Coordinates = coordinates;
        }

        public Guid Id { get; set; }

        #region Particle, Neighbors

        private Node? _lower;
        private Node? _upper;

        /// <summary>
        ///     Partikel, zu dem dieser Knoten gehört.
        /// </summary>
        public Particle Particle { get; }

        /// <inheritdoc />
        public Guid ParticleId => Particle.Id;

        public Node Upper => _upper ?? throw new InvalidNeighborhoodException(this, InvalidNeighborhoodException.Neighbor.Upper);

        public Node Lower => _lower ?? throw new InvalidNeighborhoodException(this, InvalidNeighborhoodException.Neighbor.Lower);

        Node? IRingItem<Node>.Upper
        {
            get => _upper;
            set
            {
                _upper?.ClearCache();
                _upper = value;
                ClearCache();
                _upper?.ClearCache();
            }
        }

        Node? IRingItem<Node>.Lower
        {
            get => _lower;
            set
            {
                _lower?.ClearCache();
                _lower = value;
                ClearCache();
                _lower?.ClearCache();
            }
        }

        /// <inheritdoc />
        Ring<Node>? IRingItem<Node>.Ring { get; set; }

        /// <summary>
        ///     Removes this knot from the surface.
        /// </summary>
        public void Remove()
        {
            this.Remove<Node>();
            _logger.LogTrace("{Node} removed.", ToString());
        }

        /// <summary>
        ///     Insert a knot above this.
        /// </summary>
        /// <param name="insertion"></param>
        public void InsertAbove(Node insertion)
        {
            this.InsertAbove<Node>(insertion);
            _logger.LogTrace("{Insertion} inserted above {Location}.", insertion.ToString(), ToString());
        }

        /// <summary>
        ///     Insert a knot below this.
        /// </summary>
        /// <param name="insertion"></param>
        public void InsertBelow(Node insertion)
        {
            this.InsertBelow<Node>(insertion);
            _logger.LogTrace("{Insertion} inserted below {Location}.", insertion.ToString(), ToString());
        }

        #endregion

        #region CurrentGeometry

        private PolarPoint? _coordinates;
        private ToUpperToLowerAngle? _angleDistance;
        private ToUpperToLower? _surfaceDistance;
        private ToUpperToLowerAngle? _surfaceRadiusAngle;

        /// <summary>
        ///     Koordinaten des Punktes mit Basis auf <see cref="ParticleModel.Particle.LocalCoordinateSystem" />
        /// </summary>
        public PolarPoint Coordinates
        {
            get => _coordinates ?? throw new PropertyNotSetException(nameof(Coordinates), ToString(true));
            internal set
            {
                _coordinates = value;
                _coordinates.System = Particle.LocalCoordinateSystem;
                ClearCache();
                _upper?.ClearCache();
                _lower?.ClearCache();
            }
        }

        /// <inheritdoc />
        public AbsolutePoint AbsoluteCoordinates => Coordinates.Absolute;

        /// <summary>
        ///     Winkeldistanz zu den Nachbarknoten (Größe des kürzesten Winkels).
        /// </summary>
        public ToUpperToLowerAngle AngleDistance => _angleDistance ??= new ToUpperToLowerAngle(
            Coordinates.AngleTo(Upper.Coordinates),
            Coordinates.AngleTo(Lower.Coordinates)
        );

        /// <summary>
        ///     Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
        /// </summary>
        public ToUpperToLower SurfaceDistance => _surfaceDistance ??= new ToUpperToLower(
            CosLaw.C(Upper.Coordinates.R, Coordinates.R, AngleDistance.ToUpper),
            CosLaw.C(Lower.Coordinates.R, Coordinates.R, AngleDistance.ToLower)
        );

        /// <summary>
        ///     Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
        /// </summary>
        public ToUpperToLowerAngle SurfaceRadiusAngle => _surfaceRadiusAngle ??= new ToUpperToLowerAngle(
            CosLaw.Gamma(SurfaceDistance.ToUpper, Coordinates.R, Upper.Coordinates.R),
            CosLaw.Gamma(SurfaceDistance.ToLower, Coordinates.R, Lower.Coordinates.R)
        );

        /// <summary>
        ///     Gesamtes Volumen der an den Knoten angrenzenden Elemente.
        /// </summary>
        public double NeighborElementsVolume => 0.5 * Coordinates.R
                                                    * (Upper.Coordinates.R * Sin(AngleDistance.ToUpper)
                                                     + Lower.Coordinates.R * Sin(AngleDistance.ToLower));

        #endregion

        #region FutureGeometry

        private PolarPoint? _futureCoordinates;
        private ToUpperToLowerAngle? _futureAngleDistance;
        private ToUpperToLower? _futureSurfaceDistance;
        private ToUpperToLowerAngle? _futureSurfaceRadiusAngle;

        /// <summary>
        ///     Koordinaten des Knotens nach Durchführung des Zeitschritts. Kann
        /// </summary>
        public PolarPoint FutureCoordinates
        {
            get => _futureCoordinates ?? throw new PropertyNotSetException(nameof(FutureCoordinates), ToString(true));
            private protected set
            {
                value.System = Particle.FutureLocalCoordinateSystem;
                _futureCoordinates = value;
                ClearFutureGeometryCache();
                _upper?.ClearFutureGeometryCache();
                _lower?.ClearFutureGeometryCache();
            }
        }

        /// <summary>
        ///     Winkeldistanz zu den Nachbarknoten (Größe des kürzesten Winkels).
        /// </summary>
        public ToUpperToLowerAngle FutureAngleDistance => _futureAngleDistance ??= new ToUpperToLowerAngle(
            FutureCoordinates.AngleTo(Upper.FutureCoordinates),
            FutureCoordinates.AngleTo(Lower.FutureCoordinates)
        );

        /// <summary>
        ///     Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
        /// </summary>
        public ToUpperToLower FutureSurfaceDistance => _futureSurfaceDistance ??= new ToUpperToLower(
            CosLaw.C(Upper.FutureCoordinates.R, FutureCoordinates.R, FutureAngleDistance.ToUpper),
            CosLaw.C(Lower.FutureCoordinates.R, FutureCoordinates.R, FutureAngleDistance.ToLower)
        );

        /// <summary>
        ///     Distanz zu den Nachbarknoten (Länge der Verbindungsgeraden).
        /// </summary>
        public ToUpperToLowerAngle FutureSurfaceRadiusAngle => _futureSurfaceRadiusAngle ??= new ToUpperToLowerAngle(
            CosLaw.Gamma(FutureSurfaceDistance.ToUpper, FutureCoordinates.R, Upper.FutureCoordinates.R),
            CosLaw.Gamma(FutureSurfaceDistance.ToLower, FutureCoordinates.R, Lower.FutureCoordinates.R)
        );

        /// <summary>
        ///     Gesamtes Volumen der an den Knoten angrenzenden Elemente.
        /// </summary>
        public double FutureNeighborElementsVolume => 0.5 * FutureCoordinates.R
                                                          * (Upper.FutureCoordinates.R * Sin(FutureAngleDistance.ToUpper)
                                                           + Lower.FutureCoordinates.R * Sin(FutureAngleDistance.ToLower));

        #endregion

        #region DiffusionCalculation

        private double? _curvature;
        private double? _deviatoricChemicalPotential;
        private double? _deviatoricVacancyConcentration;
        private ToUpperToLower? _vacancyConcentrationGradient;
        private ToUpperToLower? _diffusionalFlow;

        public abstract double InterfaceEnergy { get; }

        public abstract ToUpperToLower DiffusionCoefficient { get; }

        public virtual double Curvature
        {
            get
            {
                if (_curvature.HasValue) return _curvature.Value;
                var x1 = -Lower.Coordinates.R * Sin(AngleDistance.ToLower);
                var x3 = Upper.Coordinates.R * Sin(AngleDistance.ToUpper);
                var y1 = Lower.Coordinates.R * Cos(AngleDistance.ToLower);
                var y2 = Coordinates.R;
                var y3 = Upper.Coordinates.R * Cos(AngleDistance.ToUpper);

                // berechne Krümmung
                _curvature = -(x3 * y1 + x1 * y2 - x3 * y2 - x1 * y3) /
                             (Pow(x1, 2) * x3 - x1 * Pow(x3, 2));
                return _curvature.Value;
            }
        }

        public double SurfaceTension => -Curvature * InterfaceEnergy;

        public virtual double DeviatoricChemicalPotential => _deviatoricChemicalPotential ??=
            -SurfaceTension * Particle.Material.MolarVolume;

        public virtual double DeviatoricVacancyConcentration => _deviatoricVacancyConcentration ??=
            -Particle.Material.ThermalVacancyConcentration / Particle.Process.UniversalGasConstant / Particle.Process.Temperature *
            DeviatoricChemicalPotential;
        
        public double VacancyConcentration => Particle.Material.ThermalVacancyConcentration + DeviatoricVacancyConcentration;

        public virtual ToUpperToLower VacancyConcentrationGradient =>
            _vacancyConcentrationGradient ??= new ToUpperToLower(
                (Upper.DeviatoricVacancyConcentration - DeviatoricVacancyConcentration) /
                CosLaw.C(Upper.Coordinates.R, Coordinates.R, AngleDistance.ToUpper),
                (Lower.DeviatoricVacancyConcentration - DeviatoricVacancyConcentration) /
                CosLaw.C(Lower.Coordinates.R, Coordinates.R, AngleDistance.ToLower)
            );

        public ToUpperToLower DiffusionalFlow => _diffusionalFlow ??= new ToUpperToLower(
            VacancyConcentrationGradient.ToUpper * DiffusionCoefficient.ToUpper,
            VacancyConcentrationGradient.ToLower * DiffusionCoefficient.ToLower
        );

        public virtual double DiffusionalFlowBalance => -DiffusionalFlow.ToUpper - DiffusionalFlow.ToLower;

        #endregion

        #region TimeStep

        public virtual PolarVector TimeStepDisplacementVector
        {
            get => _timeStepDisplacementVector ?? throw new PropertyNotSetException(nameof(TimeStepDisplacementVector), ToString(true));
            private set => _timeStepDisplacementVector = value;
        }

        public virtual void InitFutureCoordinates()
        {
            FutureCoordinates = Coordinates.Clone();
        }

        public virtual void CalculateTimeStep(ISinteringSolverSession session)
        {
            var gamma = (Pi2 - SurfaceRadiusAngle.ToUpper.Radians - SurfaceRadiusAngle.ToLower.Radians) / 2;
            var ds = 2 * DiffusionalFlowBalance * session.TimeStepWidth / SurfaceDistance.Sum / Sin(gamma);
            var delta = gamma + SurfaceRadiusAngle.ToUpper.Radians;

            var r = CosLaw.C(ds, Coordinates.R, delta);
            var dPhi = SinLaw.Alpha(ds, r, delta);

            FutureCoordinates = new PolarPoint(Coordinates.Phi + dPhi, r, Particle.FutureLocalCoordinateSystem);
            TimeStepDisplacementVector =
                new PolarVector(Coordinates.Phi + gamma + SurfaceRadiusAngle.ToLower - Pi, ds, Particle.LocalCoordinateSystem);
        }

        public virtual void ValidateTimeStep(ISinteringSolverSession session)
        {
            if (
                Abs(FutureSurfaceRadiusAngle.ToUpper - SurfaceRadiusAngle.ToUpper) < session.SolverOptions.MaxSurfaceDisplacementAngle &&
                Abs(FutureSurfaceRadiusAngle.ToLower - SurfaceRadiusAngle.ToLower) < session.SolverOptions.MaxSurfaceDisplacementAngle
            )
                return;

            throw new InvalidTimeStepException(this, MovementToLarge);
        }

        public virtual void ApplyTimeStep()
        {
            Coordinates = _futureCoordinates ?? throw new InvalidOperationException(
                    "This node has not calculated a time step yet. Call CalculateTimeStep() before.")
                { Source = ToString() };
            _futureCoordinates = null;
            ClearFutureGeometryCache();
        }

        #endregion

        #region CacheHandling

        protected virtual void ClearCache()
        {
            ClearGeometryCache();
            ClearDiffusionCache();
        }

        protected virtual void ClearGeometryCache()
        {
            _curvature = null;
            _angleDistance = null;
            _surfaceDistance = null;
            _surfaceRadiusAngle = null;
        }

        protected virtual void ClearDiffusionCache()
        {
            _diffusionalFlow = null;
            _deviatoricChemicalPotential = null;
            _deviatoricVacancyConcentration = null;
            _vacancyConcentrationGradient = null;
        }

        protected virtual void ClearFutureGeometryCache()
        {
            _futureAngleDistance = null;
            _futureSurfaceDistance = null;
            _futureSurfaceRadiusAngle = null;
        }

        #endregion

        /// <summary>
        ///     Gibt die Repräsentation des Knotens als String zurück.
        ///     Format: "{Typname} {Id} of {Partikel}".
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"{GetType().Name} {Id.ToShortString()} @ {Coordinates.ToString("(,)", CultureInfo.InvariantCulture)} of {Particle}";

        /// <summary>
        ///     Gibt die Repräsentation des Knotens als String zurück.
        ///     Format: "{Typname} {Id} of {Partikel}".
        /// </summary>
        /// <returns></returns>
        public string ToString(bool shortVersion)
        {
            if (shortVersion)
                return
                    $"{GetType().Name} {Id.ToShortString()}";
            return ToString();
        }
    }
}