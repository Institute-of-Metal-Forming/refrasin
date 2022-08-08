using System;
using IMF.Coordinates.Polar;
using RefraSin.Core.Materials;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
    /// </summary>
    public abstract class ContactNode<TContacted> : ContactNode where TContacted : ContactNode<TContacted>
    {
        private TContacted? _contactedNode = null;
        private MaterialInterface? _materialInterface;

        /// <inheritdoc />
        protected ContactNode(Particle particle, PolarPoint coordinates) : base(particle, coordinates) { }

        /// <summary>
        /// Verbundener Knoten des anderen Partikels.
        /// </summary>
        public TContacted ContactedNode
        {
            get => _contactedNode ?? throw new NotConnectedException(this);
            set => _contactedNode = value;
        }

        public MaterialInterface MaterialInterface
        {
            get => _materialInterface ??= Particle.GetMaterialInterface(ContactedNode.Particle);
            set => _materialInterface = value;
        }

        /// <inheritdoc />
        public override Guid ContactedParticleId => ContactedNode.ParticleId;

        /// <summary>
        /// Stellt eine Verbindung zwischen zwei Knoten her (setzt die gegenseitigen <see cref="ContactedNode"/>).
        /// </summary>
        /// <param name="other">anderer Knoten</param>
        public virtual void Connect(TContacted other)
        {
            _contactedNode = other;
            other._contactedNode = (TContacted) this;
        }

        /// <summary>
        /// Löst eine Verbindung zwischen zwei Knoten (setzt die gegenseitigen <see cref="ContactedNode"/>).
        /// </summary>
        public virtual void Disconnect()
        {
            ContactedNode._contactedNode = null;
            _contactedNode = null;
        }

        /// <inheritdoc />
        public override void InitFutureCoordinates()
        {
            FutureCoordinates = Coordinates.Clone();
            ContactedNode.FutureCoordinates = ContactedNode.Coordinates.Clone();
        }

        /// <summary>
        /// Merges both contacted knots' <see cref="Node.FutureCoordinates"/> at their middle point. 
        /// </summary>
        protected void MergeFutureCoordinates()
        {
            var middleCoordinates = FutureCoordinates.Absolute
                .PointHalfWayTo(ContactedNode.FutureCoordinates.Absolute);
            FutureCoordinates = new PolarPoint(middleCoordinates, Particle.FutureLocalCoordinateSystem);
            ContactedNode.FutureCoordinates =
                new PolarPoint(middleCoordinates, ContactedNode.Particle.FutureLocalCoordinateSystem);

            ClearFutureGeometryCache();
            ContactedNode.ClearFutureGeometryCache();
        }

        /// <summary>
        /// Sets the contacted knot's <see cref="Node.FutureCoordinates"/> to the same point as this knot.
        /// </summary>
        public void PullContactedNodesFutureCoordinates()
        {
            ContactedNode.FutureCoordinates = new PolarPoint(FutureCoordinates, ContactedNode.Particle.FutureLocalCoordinateSystem);
            ContactedNode.ClearFutureGeometryCache();
        }

        public class NotConnectedException : InvalidOperationException
        {
            public NotConnectedException(ContactNode<TContacted> sourceNode)
            {
                SourceNode = sourceNode;
                Message = $"Contact node {sourceNode} is not connected another node.";
            }

            public override string Message { get; }
            public ContactNode<TContacted> SourceNode { get; }
        }
    }

    /// <summary>
    /// Abstrakte Basisklasse für Oberflächenknoten eines Partikels, welche Kontakt zur Oberfläche eines anderen partiekls haben.
    /// </summary>
    public abstract class ContactNode : Node, IContactNode
    {
        private double _contactStress;
        private double? _deviatoricChemicalPotential;

        /// <inheritdoc />
        protected ContactNode(Particle particle, PolarPoint coordinates) : base(particle, coordinates) { }

        /// <inheritdoc />
        public abstract Guid ContactedParticleId { get; }

        /// <summary>
        /// Spannung durch den Kontakt.
        /// </summary>
        public virtual double ContactStress
        {
            get => _contactStress;
            set
            {
                _contactStress = value;
                ClearDiffusionCache();
            }
        }

        /// <summary>
        /// Spannung durch den Kontakt im vorigen Zeitschritt.
        /// </summary>
        public double PastContactStress { get; set; }

        /// <inheritdoc />
        public abstract double Transfer { get; }

        /// <inheritdoc />
        public override double DeviatoricChemicalPotential => _deviatoricChemicalPotential ??=
            -(SurfaceTension + ContactStress) * Particle.Material.MolarVolume;

        /// <inheritdoc />
        public override void ApplyTimeStep()
        {
            PastContactStress = ContactStress;
            base.ApplyTimeStep();
        }
        
        /// <inheritdoc />
        protected override void ClearDiffusionCache()
        {
            _deviatoricChemicalPotential = null;
            base.ClearDiffusionCache();
        }
    }
}