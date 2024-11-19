namespace RefraSin.MaterialData;

public interface ISubstanceProperties
{
    /// <summary>
    /// Density.
    /// </summary>
    double Density { get; }

    /// <summary>
    /// Molar volume.
    /// </summary>
    double MolarVolume { get; }

    /// <summary>
    /// Molar mass.
    /// </summary>
    double MolarMass { get; }
}
