namespace RefraSin.ParticleModel.Nodes;

public interface INodeMaterialProperties
{
    /// <summary>
    ///  Surface resp. interface energy of the surface lines to the neighbor nodes.
    /// </summary>
    public ToUpperToLower<double> InterfaceEnergy { get; }

    /// <summary>
    /// Diffusion coefficient at the surface lines to the neighbor nodes.
    /// </summary>
    public ToUpperToLower<double> InterfaceDiffusionCoefficient { get; }
}