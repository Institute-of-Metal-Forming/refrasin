namespace RefraSin.ParticleModel;

public interface INodeMaterialProperties
{
    /// <summary>
    ///  Surface resp. interface energy of the surface lines to the neighbor nodes.
    /// </summary>
    public ToUpperToLower SurfaceEnergy { get; }

    /// <summary>
    /// Diffusion coefficient at the surface lines to the neighbor nodes.
    /// </summary>
    public ToUpperToLower SurfaceDiffusionCoefficient { get; }

    /// <summary>
    /// Coefficient for volume transfer to/from the environment of the particle.
    /// </summary>
    public double TransferCoefficient { get; }
}