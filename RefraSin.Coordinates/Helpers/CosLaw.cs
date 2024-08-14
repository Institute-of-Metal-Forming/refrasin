using static System.Math;
using static RefraSin.Coordinates.Constants;
using static RefraSin.Coordinates.Helpers.MathExt;

namespace RefraSin.Coordinates.Helpers;

/// <summary>
///     Implementiert den Cosinussatz am allgemeinen Dreieck.
/// </summary>
public static class CosLaw
{
    /// <summary>
    ///     Berechnet die Länge der Hypothenuse
    /// </summary>
    /// <param name="a">Länge Kathete</param>
    /// <param name="b">Länge Kathete</param>
    /// <param name="gamma">eingeschlossener Winkel</param>
    /// <returns>Länge Hypothenuse</returns>
    public static double C(double a, double b, Angle gamma) =>
        Sqrt(Pow(a, 2) + Pow(b, 2) - 2 * a * b * Cos(gamma));

    /// <summary>
    ///     Berechnet die möglichen Längen einer Kathete
    /// </summary>
    /// <param name="b">Länge Kathete</param>
    /// <param name="c">Länge Hypothenuse</param>
    /// <param name="gamma">eingeschlossener Winkel</param>
    /// <returns>Tupel der möglichen Längen der Kathete</returns>
    public static (double, double) A(double b, double c, Angle gamma)
    {
        var p1 = 2 * b * Cos(gamma);
        var p2 = Sqrt2 * Sqrt(-Squared(b) + 2 * Squared(c) + Squared(b) * Cos(2 * gamma));
        return (0.5 * (p1 - p2), 0.5 * (p1 + p2));
    }

    /// <summary>
    ///     Berechnet den eingeschlossenen Winkel
    /// </summary>
    /// <param name="a">Länge Kathete</param>
    /// <param name="b">Länge Kathete</param>
    /// <param name="c">Länge Hypothenuse</param>
    /// <returns>eingeschlossener Winkel</returns>
    public static Angle Gamma(double a, double b, double c) =>
        Acos((Squared(a) + Squared(b) - Squared(c)) / (2 * a * b));
}
