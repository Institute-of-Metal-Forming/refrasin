using System;

namespace RefraSin.Core.ParticleModel.States
{
    /// <summary>
    /// Stellt den Zustand eines Kontaktknotens dar.
    /// </summary>
    public abstract class ContactNodeState : NodeState, IContactNode
    {
        /// <summary>
        /// Kopierkonstruktor.
        /// </summary>
        /// <param name="template">Vorlage</param>
        public ContactNodeState(IContactNode template) : base(template)
        {
            ContactStress = template.ContactStress;
            ContactedParticleId = template.ContactedParticleId;
            Transfer = template.Transfer;
        }

        /// <inheritdoc />
        public double ContactStress { get; set; }

        /// <inheritdoc />
        public Guid ContactedParticleId { get; set; }

        /// <inheritdoc />
        public double Transfer { get; }
    }
}