using System;

namespace RefraSin.Coordinates.Helpers;

/// <summary>
///     Erweiterung für <see cref="IEquatable{T}" /> mit Genauigkeitsschranke für double-Vergleiche.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPrecisionEquatable<T> : IEquatable<T>
{
    /// <summary>
    ///     Prüft auf Gleichheit unter der angegebenen Genauigkeit.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="precision">maximal gültige Abweichung</param>
    /// <returns></returns>
    public bool Equals(T other, double precision);

    /// <summary>
    ///     Prüft auf Gleichheit unter der angegebenen Genauigkeit.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="digits">Anzahl signifikanter Stellen</param>
    /// <returns></returns>
    public bool Equals(T other, int digits);
}