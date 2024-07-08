namespace RefraSin.MaterialData;

public record SubstanceProperties(double Density, double MolarMass) : ISubstanceProperties
{
    /// <inheritdoc />
    public double MolarVolume { get; } = MolarMass / Density;
}
