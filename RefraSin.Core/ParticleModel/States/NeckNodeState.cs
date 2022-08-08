using System;

namespace RefraSin.Core.ParticleModel.States
{
    public class NeckNodeState : ContactNodeState, INeckNode
    {
        /// <summary>
        /// Kopierkonstruktor.
        /// </summary>
        /// <param name="template">Vorlage</param>
        public NeckNodeState(INeckNode template) : base(template)
        {
            OppositeNeckNodeId = template.OppositeNeckNodeId;
        }

        public Guid OppositeNeckNodeId { get; set; }
    }
}