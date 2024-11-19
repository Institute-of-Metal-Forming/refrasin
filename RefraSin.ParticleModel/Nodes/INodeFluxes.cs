namespace RefraSin.ParticleModel.Nodes;

/// <summary>
/// Interface for flux data on a node.
/// </summary>
public interface INodeFluxes
{
    /// <summary>
    /// Flux along the interface resp. surface to the neighbor nodes.
    /// </summary>
    public ToUpperToLower<double> InterfaceFlux { get; }

    /// <summary>
    /// Flux through the volume to the neighbor nodes.
    /// </summary>
    public ToUpperToLower<double> VolumeFlux { get; }

    /// <summary>
    /// Transfer flux cross the interface (f.e. to environment or another particle).
    /// </summary>
    public double TransferFlux { get; }
}
