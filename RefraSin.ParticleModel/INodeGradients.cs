namespace RefraSin.ParticleModel;

public interface INodeGradients
{
    /// <summary>
    /// Gradient of Gibbs energy for node shifting in normal and tangential direction.
    /// </summary>
    public NormalTangential<double> GibbsEnergyGradient { get; }

    /// <summary>
    /// Gradient of volume for node shifting in normal and tangential direction.
    /// </summary>
    public NormalTangential<double> VolumeGradient { get; }
}