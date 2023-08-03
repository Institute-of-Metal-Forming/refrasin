using System.Collections.Generic;
using static System.Math;

namespace RefraSin.Coordinates.Helpers;

/// <summary>
///     Mathematische Erweiterungsmethoden.
/// </summary>
public static class Extensions
{
    /// <summary>
    ///     Gibt das Quadrat des Arguments zurück.
    /// </summary>
    /// <param name="x">Zahl</param>
    /// <returns>x^2</returns>
    public static double Squared(this double x) => Pow(x, 2);

    /// <summary>
    ///     Gibt die dritte Potenz des Arguments zurück.
    /// </summary>
    /// <param name="x">Zahl</param>
    /// <returns>x^3</returns>
    public static double Cubed(this double x) => Pow(x, 3);

    /// <summary>
    ///     Berechnet das ugf. Integral einer Funktion, die nur als Wertetabelle gegeben ist. Dabei wird zwischen zwei
    ///     Wertepaaren immer die Trapezregel angewendet.
    /// </summary>
    /// <param name="values">Wertetabelle</param>
    /// <returns></returns>
    public static double Integrate(this IEnumerable<(double x, double y)> values)
    {
        (double x, double y)? lastTuple = null;
        var integral = 0.0;
        foreach (var tuple in values)
        {
            if (lastTuple.HasValue) integral += 0.5 * (tuple.y + lastTuple.Value.y) * (tuple.x - lastTuple.Value.x);

            lastTuple = tuple;
        }

        return integral;
    }
}