namespace RefraSin.Core.ParticleModel.HelperTypes;

/// <summary>
/// Structure encapsulating quantities with two components, one in normal direction, one in tangential.
/// </summary>
public struct NormalTangential
{
    public NormalTangential(double normal, double tangential)
    {
        Normal = normal;
        Tangential = tangential;
    }

    /// <summary>
    /// Normal component.
    /// </summary>
    public double Normal { get; }

    /// <summary>
    /// Tangential component.
    /// </summary>
    public double Tangential { get; }
}