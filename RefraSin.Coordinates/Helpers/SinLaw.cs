using System.Runtime.CompilerServices;

namespace RefraSin.Coordinates.Helpers;

/// <summary>
///     Implementiert den Sinussatz am allgemeinen Dreieck.
/// </summary>
public static class SinLaw
{
    /// <summary>
    ///     Berechnet die Länge einer Seite
    /// </summary>
    /// <param name="b">Länge der anderen Seite</param>
    /// <param name="alpha">Winkel ggü. der gesuchten Seite</param>
    /// <param name="beta">Winkel ggü. der anderen Seite</param>
    /// <returns>Länge der Seite ggü. alpha</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double A(double b, Angle alpha, Angle beta) =>
        b * Math.Sin(alpha) / Math.Sin(beta);

    /// <summary>
    ///     Berechnet einen Winkel
    /// </summary>
    /// <param name="a">Länge der Seite ggü. des gesuchten Winkels</param>
    /// <param name="b">Länge der anderen Seite</param>
    /// <param name="beta">Winkel ggü. der anderen Seite</param>
    /// <returns>Winkel ggü. a</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Angle Alpha(double a, double b, Angle beta) => Math.Asin(a / b * Math.Sin(beta));
}
