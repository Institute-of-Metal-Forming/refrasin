using RefraSin.Coordinates;

namespace RefraSin.Core.ParticleModel.HelperTypes;

/// <summary>
/// Stellt ein double-Tupel mit Werten einer nach oben und nach unten vorhandenen Eigenschaft dar.
/// </summary>
public readonly struct ToUpperToLower
{
    /// <summary>
    /// Kontruktor.
    /// </summary>
    /// <param name="toUpper">Wert nach oben</param>
    /// <param name="toLower">Wert nach unten</param>
    public ToUpperToLower(double toUpper, double toLower)
    {
        ToUpper = toUpper;
        ToLower = toLower;
        Sum = toUpper + toLower;
    }

    /// <summary>
    /// Wert nach oben
    /// </summary>
    public readonly double ToUpper;

    /// <summary>
    /// Wert nach unten
    /// </summary>
    public readonly double ToLower;

    /// <summary>
    /// Summe beider Werte
    /// </summary>
    public readonly double Sum;

    /// <inheritdoc />
    public override string ToString() => $"ToUpper: {ToUpper}, ToLower: {ToLower}, Sum: {Sum}";
}

/// <summary>
/// Stellt ein double-Tupel mit Werten einer nach oben und nach unten vorhandenen Eigenschaft dar.
/// </summary>
public readonly struct ToUpperToLowerAngle
{
    /// <summary>
    /// Kontruktor.
    /// </summary>
    /// <param name="toUpper">Wert nach oben</param>
    /// <param name="toLower">Wert nach unten</param>
    public ToUpperToLowerAngle(Angle toUpper, Angle toLower)
    {
        ToUpper = toUpper;
        ToLower = toLower;
        Sum = toUpper + toLower;
    }

    /// <summary>
    /// Wert nach oben
    /// </summary>
    public readonly Angle ToUpper;

    /// <summary>
    /// Wert nach unten
    /// </summary>
    public readonly Angle ToLower;

    /// <summary>
    /// Summe beider Werte
    /// </summary>
    public readonly Angle Sum;

    /// <inheritdoc />
    public override string ToString() => $"ToUpper: {ToUpper}, ToLower: {ToLower}, Sum: {Sum}";
}