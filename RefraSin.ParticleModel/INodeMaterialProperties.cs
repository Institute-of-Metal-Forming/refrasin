namespace RefraSin.ParticleModel;

public interface INodeMaterialProperties
{
    /// <summary>
    ///  Surface resp. interface energy of the surface lines to the neighbor nodes.
    /// </summary>
    public ToUpperToLower<double> SurfaceEnergy { get; }

    /// <summary>
    /// Diffusion coefficient at the surface lines to the neighbor nodes.
    /// </summary>
    public ToUpperToLower<double> SurfaceDiffusionCoefficient { get; }
}