using System;

namespace RefraSin.Core.ParticleModel.Interfaces;

/// <summary>
/// Interface für Halsknoten.
/// </summary>
public interface INeckNode : IContactNode
{
    public Guid OppositeNeckNodeId { get; }
}