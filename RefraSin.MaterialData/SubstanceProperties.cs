namespace RefraSin.MaterialData;

public record SubstanceProperties(double Density, double MolarMass, double MolarVolume)
    : ISubstanceProperties
{
    public static SubstanceProperties FromDensityAndMolarMass(double density, double molarMass)
    {
        return new SubstanceProperties(density, molarMass, molarMass / density);
    }

    public static SubstanceProperties FromDensityAndMolarVolume(double density, double molarVolume)
    {
        return new SubstanceProperties(density, molarVolume * density, molarVolume);
    }

    public static SubstanceProperties FromMolarVolumeAndMolarMass(
        double molarVolume,
        double molarMass
    )
    {
        return new SubstanceProperties(molarMass / molarVolume, molarMass, molarVolume);
    }
}
