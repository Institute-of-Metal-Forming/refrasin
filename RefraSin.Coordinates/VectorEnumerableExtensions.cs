using System;
using System.Collections.Generic;

namespace RefraSin.Coordinates;

/// <summary>
///     Provides extension methods for enumerables of vectors.
/// </summary>
public static class VectorEnumerableExtensions
{
    /// <summary>
    ///     Computes the vectorial sum of a sequence of vectors.
    /// </summary>
    /// <param name="vectors">sequence</param>
    public static TVector Sum<TVector>(this IEnumerable<TVector> vectors) where TVector : IVector
    {
        using var enumerator = vectors.GetEnumerator();
        if (!enumerator.MoveNext()) throw new InvalidOperationException("Enumerable cannot be empty.");
        var vectorSum = enumerator.Current;

        while (enumerator.MoveNext()) vectorSum = (TVector) vectorSum.Add(enumerator.Current);

        return vectorSum;
    }

    /// <summary>
    ///     Computes the average vector of a sequence of vectors.
    /// </summary>
    /// <param name="vectors">sequence</param>
    public static TVector Average<TVector>(this IEnumerable<TVector> vectors) where TVector : IVector
    {
        using var enumerator = vectors.GetEnumerator();
        if (!enumerator.MoveNext()) throw new InvalidOperationException("Enumerable can not be empty.");
        var vectorSum = enumerator.Current;
        var count = 1;

        while (enumerator.MoveNext())
        {
            vectorSum = (TVector) vectorSum.Add(enumerator.Current);
            count++;
        }

        vectorSum.ScaleBy(1.0 / count);
        return vectorSum;
    }

    /// <summary>
    ///     Computes the direction of the average vector of a sequence of vectors.
    /// </summary>
    /// <param name="vectors">sequence</param>
    public static TVector AverageDirection<TVector>(this IEnumerable<TVector> vectors) where TVector : IVector
    {
        using var enumerator = vectors.GetEnumerator();
        if (!enumerator.MoveNext()) throw new InvalidOperationException("Enumerable can not be empty.");
        var vectorSum = enumerator.Current;

        while (enumerator.MoveNext()) vectorSum = (TVector) vectorSum.Add(enumerator.Current);

        vectorSum.ScaleBy(1 / vectorSum.Norm);
        return vectorSum;
    }
}