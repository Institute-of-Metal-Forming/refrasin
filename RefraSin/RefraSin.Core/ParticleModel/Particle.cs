using System;
using System.Collections.Generic;
using System.Linq;
using IMF.Coordinates;
using IMF.Coordinates.Absolute;
using IMF.Coordinates.Polar;
using IMF.Enumerables;
using IMF.Utils;
using RefraSin.Core.Materials;
using RefraSin.Core.SinteringProcesses;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Stellt ein Pulverpartikel dar.
    /// </summary>
    public class Particle : IParticle, ITreeItem<Particle>
    {
        private static readonly PolarCoordinateSystem DefaultCenterCoordinatesSystem = new() { Label = "default center coordinates system" };

        private static readonly PolarCoordinateSystem DefaultFutureCenterCoordinatesSystem =
            new() { Label = "default future center coordinates system" };

        private PolarPoint _centerCoordinates = null!;
        private PolarPoint? _futureCenterCoordinates;

        /// <summary>
        /// Konstruktor, welcher die Oberfläche nach einem Cosinus-Ansatz erstellt.
        /// </summary>
        /// <param name="surfaceNodeSource">delegate that serves surface knots for this particle</param>
        /// <param name="material">Materialdaten</param>
        ///  <param name="process">reference to the sintering process instance</param>
        public Particle(SurfaceNodeSource surfaceNodeSource, Material material, ISinteringProcess process)
        {
            Process = process;
            Id = Guid.NewGuid();
            Children = new TreeChildrenCollection<Particle>(this);
            CenterCoordinates = new PolarPoint();
            LocalCoordinateSystem = new PolarCoordinateSystem
            {
                Label = $"local system of {this}",
                Origin = new AbsolutePoint(),
                OriginSource = () => CenterCoordinates,
                RotationAngleSource = () => RotationAngle,
                AngleReductionDomain = Angle.ReductionDomain.AllPositive
            };
            FutureLocalCoordinateSystem = new()
            {
                Label = $"future local system of {this}",
                Origin = new AbsolutePoint(),
                OriginSource = () => FutureCenterCoordinates,
                RotationAngleSource = () => FutureRotationAngle,
                AngleReductionDomain = Angle.ReductionDomain.AllPositive
            };
            Material = material;

            Surface = new ParticleSurface(surfaceNodeSource(this));
        }

        /// <summary>
        /// Aufzählung der Oberflächenknoten.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public ParticleSurface Surface { get; }

        /// <summary>
        /// Materialdaten.
        /// </summary>
        public Material Material { get; }

        public MaterialInterface GetMaterialInterface(Particle other) => Process.MaterialInterfaces[(Material, other.Material)];

        /// <summary>
        /// Drehwinkel des Partikels nach dem Zeitschritt.
        /// </summary>
        public Angle? FutureRotationAngle { get; set; }

        /// <summary>
        /// Koordinaten des Ursprungs des lokalen Koordinatensystem ausgedrückt im Koordinatensystem des <see cref="Parent"/> nach Durchführen des Zeitschritts.
        /// </summary>
        public PolarPoint? FutureCenterCoordinates
        {
            get => _futureCenterCoordinates;
            set
            {
                _futureCenterCoordinates = value;
                if (_futureCenterCoordinates != null)
                {
                    _futureCenterCoordinates.System = DefaultFutureCenterCoordinatesSystem;
                    _futureCenterCoordinates.SystemSource = () => Parent?.FutureLocalCoordinateSystem;
                }
            }
        }

        /// <summary>
        /// Änderung der Distanz zum Parent im letzten Zeitschritt.
        /// </summary>
        public double? MovementDistanceOfLastTimeStep { get; set; }

        /// <summary>
        /// Lokales Koordinatensystem des Partikels. Bearbeitung über <see cref="CenterCoordinates"/> und <see cref="RotationAngle"/>. Sollte nicht direkt verändert werden!!!
        /// </summary>
        internal PolarCoordinateSystem LocalCoordinateSystem { get; }

        /// <summary>
        /// Lokales Koordinatensystem des Partikels für Koordinaten nach dem Zeitschritt. Bearbeitung über <see cref="FutureCenterCoordinates"/> und <see cref="FutureRotationAngle"/>. Sollte nicht direkt verändert werden!!!
        /// </summary>
        internal PolarCoordinateSystem FutureLocalCoordinateSystem { get; }

        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        public IReadOnlyList<INeck> Necks
        {
            get
            {
                var necks = new List<Neck>();
                foreach (var node in Surface)
                {
                    if (node is NeckNode neckNode)
                        if (node.Upper is GrainBoundaryNode)
                            necks.Add(new Neck(neckNode));
                }

                return necks;
            }
        }

        /// <summary>
        /// Drehwinkel des Partikels.
        /// </summary>
        public Angle RotationAngle { get; set; }

        /// <inheritdoc />
        public AbsolutePoint AbsoluteCenterCoordinates => CenterCoordinates.Absolute;

        /// <inheritdoc />
        public IReadOnlyList<INode> SurfaceNodes => Surface.ToArray();

        /// <summary>
        /// Koordinaten des Ursprungs des lokalen Koordinatensystem ausgedrückt im Koordinatensystem des <see cref="Parent"/>
        /// </summary>
        public PolarPoint CenterCoordinates
        {
            get => _centerCoordinates;
            set
            {
                _centerCoordinates = value;
                _centerCoordinates.System = DefaultCenterCoordinatesSystem;
                _centerCoordinates.SystemSource = () => Parent?.LocalCoordinateSystem;
            }
        }

        /// <summary>
        /// Übergeordnetes Partikel dieses Partikels in der Baumanordnung.
        /// </summary>
        public Particle? Parent { get; set; }

        /// <summary>
        /// Untergeordnete Partikel dieses Partikels in der Baumanordnung.
        /// </summary>
        public TreeChildrenCollection<Particle> Children { get; }

        /// <summary>
        /// The process this particle belongs to.
        /// </summary>
        public ISinteringProcess Process { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{GetType().Name} {Id.ToShortString()}";
        }

        /// <summary>
        /// Überträgt alle zukünftigen Eigenschaften (Future*) des Partikels und seiner Oberflächenknoten auf ihre Pendants.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void ApplyTimeStep()
        {
            if (FutureCenterCoordinates == null || !FutureRotationAngle.HasValue)
                throw new InvalidOperationException($"Time step was not calculated.");

            CenterCoordinates = FutureCenterCoordinates;
            RotationAngle = FutureRotationAngle.Value;
            foreach (var node in Surface)
                node.ApplyTimeStep();

            FutureCenterCoordinates = null;
        }
    }
}