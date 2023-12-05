namespace RefraSin.ParticleModel;

public interface INodeGradients
{
    /// <summary>
    /// Gradient of Gibbs energy for node shifting in normal and tangential direction.
    /// </summary>
    public NormalTangential GibbsEnergyGradient { get; }

    /// <summary>
    /// Gradient of volume for node shifting in normal and tangential direction.
    /// </summary>
    public NormalTangential VolumeGradient { get; }
}