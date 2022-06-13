using System;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Interface für Halsknoten.
    /// </summary>
    public interface INeckNode : IContactNode
    {
        public Guid OppositeNeckNodeId { get; }
    }
}