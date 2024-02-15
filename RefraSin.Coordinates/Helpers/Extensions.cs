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
    /// Determines if two double values are equal within a precision.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="other"></param>
    /// <param name="precision">absolute precision</param>
    /// <returns></returns>
    public static bool IsClose(this double self, double other, double precision)
    {
        if (double.IsInfinity(self) || double.IsInfinity(other))
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return self == other;

        if (double.IsNaN(self) || double.IsNaN(other))
            return false;
        
        return Abs(self - other) < precision;
    }
}