using System;
using System.Collections.Generic;

namespace RefraSin.Enumerables;

/// <summary>
///     Provides some extra functionality on <see cref="Tree{T}" />.
/// </summary>
public static class TreeExtensions
{
    /// <summary>
    /// Maps a tree to another according to a selector function.
    /// </summary>
    /// <param name="source">source tree</param>
    /// <param name="selector">function that creates the items of the result tree</param>
    /// <typeparam name="TSource">type of the source tree items</typeparam>
    /// <typeparam name="TResult">type of the result tree items</typeparam>
    /// <returns>the built result tree</returns>
    public static Tree<TResult> TreeMap<TSource, TResult>(
        this Tree<TSource> source,
        Func<TSource, TResult> selector
    )
        where TSource : class, ITreeItem<TSource>
        where TResult : class, ITreeItem<TResult>
    {
        var sourceRoot = source.Root;
        var resultRoot = selector(sourceRoot);

        var queue = new Queue<(TSource sourceItem, TResult resultItem)>();
        queue.Enqueue((sourceRoot, resultRoot));

        while (true)
        {
            var result = queue.TryDequeue(out var parents);
            if (!result)
                break;

            foreach (var sourceItem in parents.sourceItem.Children)
            {
                var resultItem = selector(sourceItem);
                parents.resultItem.Children.Add(resultItem);
                queue.Enqueue((sourceItem, resultItem));
            }
        }

        return resultRoot.GetTree();
    }
}
