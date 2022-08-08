using System;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Schnittstelle für Knoten, welche einen Kontakt herstellen.
    /// </summary>
    public interface IContactNode : INode
    {
        /// <summary>
        /// Spannung durch berührung der Oberlächen.
        /// </summary>
        public double ContactStress { get; }
        
        /// <summary>
        /// Id des Partikels, welches dieser Knoten berührt.
        /// </summary>
        public Guid ContactedParticleId { get; }
        
        /// <summary>
        /// Volume transfer across the grain boundary.
        /// </summary>
        public double Transfer { get; }
    }
}