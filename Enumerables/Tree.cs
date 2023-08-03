using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RefraSin.Enumerables;

/// <summary>
///     Represents a tree structure.
///     Supports enumeration and execution of actions on elements.
/// </summary>
/// <typeparam name="TTreeItem">type of the items, must implement <see cref="ITreeItem{TTreeItem}" /></typeparam>
public class Tree<TTreeItem> : IEnumerable<TTreeItem> where TTreeItem : class, ITreeItem<TTreeItem>
{
    /// <summary>
    ///     Creates a new tree with the given root element.
    /// </summary>
    /// <param name="root"></param>
    public Tree(TTreeItem root)
    {
        Root = root;
    }

    /// <summary>
    ///     Root element of the tree.
    /// </summary>
    public TTreeItem Root { get; }

    /// <inheritdoc />
    public IEnumerator<TTreeItem> GetEnumerator() => Enumerate().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     Iterates over the tree executing an action at each element.
    ///     Creates a new Task at every branch.
    /// </summary>
    /// <param name="action"></param>
    public Task ForEachParallelAsync(Action<TTreeItem> action)
    {
        void WalkFurther(TTreeItem current)
        {
            action(current);

            var children = current.Children.ToArray();
            var first = children.FirstOrDefault();
            if (first == null)
                return;

            // spawn tasks at branches
            foreach (var child in children.Skip(1))
                Task.Factory.StartNew(() => WalkFurther(child), TaskCreationOptions.AttachedToParent);

            // current task takes first child
            WalkFurther(first);
        }

        return Task.Factory.StartNew(() => WalkFurther(Root)); // start first task
    }

    /// <summary>
    ///     Iterates over the tree executing an action at each element.
    ///     Works completely sequentially.
    /// </summary>
    /// <param name="action"></param>
    public void ForEach(Action<TTreeItem> action)
    {
        var queue = new Queue<TTreeItem>();
        queue.Enqueue(Root);

        while (queue.Count > 0)
        {
            var el = queue.Dequeue();
            action(el);
            foreach (var child in el.Children)
                queue.Enqueue(child);
        }
    }

    /// <summary>
    ///     Iterates over the tree executing an action at each element.
    ///     Works asynchronous but sequentially.
    /// </summary>
    /// <param name="action"></param>
    public Task ForEachAsync(Action<TTreeItem> action)
    {
        return Task.Factory.StartNew(() => ForEach(action));
    }

    /// <summary>
    ///     Enumerates all elements of the tree.
    /// </summary>
    public IEnumerable<TTreeItem> Enumerate()
    {
        var queue = new Queue<TTreeItem>();
        queue.Enqueue(Root);

        while (queue.Count > 0)
        {
            var el = queue.Dequeue();
            yield return el;
            foreach (var child in el.Children)
                queue.Enqueue(child);
        }
    }
}