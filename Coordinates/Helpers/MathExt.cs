using static System.Math;

namespace RefraSin.Coordinates.Helpers;

/// <summary>
///     Einige statische Konstanten und Methoden in Erweiterung von System.Math
/// </summary>
public static class MathExt
{
    /// <summary>
    ///     Gibt das Quadrat des Arguments zurück.
    /// </summary>
    /// <param name="x">Zahl</param>
    /// <returns>x^2</returns>
    public static double Squared(double x) => Pow(x, 2);

    /// <summary>
    ///     Gibt die dritte Potenz des Arguments zurück.
    /// </summary>
    /// <param name="x">Zahl</param>
    /// <returns>x^3</returns>
    public static double Cubed(double x) => Pow(x, 3);

    /// <summary>
    ///     Gibt den Flächeninhalt des durch <paramref name="a" />, <paramref name="b" /> und <paramref name="gamma" />
    ///     aufgespannten Dreiecks zurück.
    /// </summary>
    /// <param name="a">erste Seite</param>
    /// <param name="b">zweite Seite</param>
    /// <param name="gamma">eingeschlossener Winkel</param>
    /// <returns>Flächeninhalt</returns>
    public static double TriangleArea(double a, double b, double gamma) => 0.5 * a * b * Sin(gamma);
}