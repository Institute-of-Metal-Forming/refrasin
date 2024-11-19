#nullable enable
using NUnit.Framework;

namespace RefraSin.Enumerables.Test;

[TestFixture]
public class EnumerablesTest
{
    private class RingItem : IRingItem<RingItem>
    {
        public RingItem(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public RingItem? Upper { get; set; }
        public RingItem? Lower { get; set; }
        public Ring<RingItem>? Ring { get; set; }

        public override string ToString() => Name;
    }

    [Test]
    public void RingTest()
    {
        var root = new RingItem("root");

        var ring = new Ring<RingItem> { root };

        var no1 = new RingItem("no1");
        root.InsertAbove(no1);
        var no2 = new RingItem("no2");
        root.InsertAbove(no2);
        var no3 = new RingItem("no3");
        root.InsertBelow(no3);
        var toReplace = new RingItem("toReplace");
        root.InsertBelow(toReplace);
        var no4 = new RingItem("no4");
        root.InsertBelow(no4);

        Assert.That(
            ring.Select(i => i.Name)
                .Zip(
                    new[] { "root", "no2", "no1", "no3", "toReplace", "no4" },
                    (first, second) => (first, second)
                )
                .All(t => t.first == t.second)
        );

        toReplace.Replace(new RingItem("new1"));

        Assert.That(
            ring.Select(i => i.Name)
                .Zip(
                    new[] { "root", "no2", "no1", "no3", "new1", "no4" },
                    (first, second) => (first, second)
                )
                .All(t => t.first == t.second)
        );

        root.Replace(toReplace);

        Assert.That(
            ring.Select(i => i.Name)
                .Zip(
                    new[] { "toReplace", "no2", "no1", "no3", "new1", "no4" },
                    (first, second) => (first, second)
                )
                .All(t => t.first == t.second)
        );

        toReplace.Remove();

        Assert.That(
            ring.Select(i => i.Name)
                .Zip(
                    new[] { "no2", "no1", "no3", "new1", "no4" },
                    (first, second) => (first, second)
                )
                .All(t => t.first == t.second)
        );

        foreach (var item in ring)
        {
            Console.WriteLine(item);
        }

        Assert.That(
            ring.GetSegment(no1, no4)
                .Select(i => i.Name)
                .Zip(new[] { "no1", "no3", "new1", "no4" }, (first, second) => (first, second))
                .All(t => t.first == t.second)
        );

        no3.Lower = no4;

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = ring.ToArray();
        });

        no3.Lower = no1;

        ring = new Ring<RingItem>(ring.Select(i => new RingItem(i.Name)));

        Assert.That(
            ring.Select(i => i.Name)
                .Zip(
                    new[] { "no2", "no1", "no3", "new1", "no4" },
                    (first, second) => (first, second)
                )
                .All(t => t.first == t.second)
        );
    }

    [Test]
    public void TreeTest()
    {
        var root = new TreeItem<string>("root")
        {
            Children =
            {
                new("child1") { Children = { new("child11"), new("child12"), new("child13") } },
                new("child2") { Children = { new("child21"), new("child22"), new("child23") } },
                new("child3") { Children = { new("child31"), new("child32"), new("child33") } },
            },
        };

        var tree = new Tree<TreeItem<string>>(root);

        tree.ForEach(i => i.Value += "ext");

        tree.ForEachAsync(i => i.Value += "ext");

        tree.ForEachParallelAsync(i => i.Value += "ext");

        tree.ForEach(i => Console.WriteLine(i));
    }

    [Test]
    public void NullableTreeTest()
    {
        var root = new TreeItem<double?>(1.0)
        {
            Children =
            {
                new(1.1) { Children = { new(1.11), new(1.12), new(1.13) } },
                new(1.2) { Children = { new(1.21), new(1.22), new(1.23) } },
                new(null) { Children = { new(1.31), new(1.32), new(1.33) } },
            },
        };

        var tree = new Tree<TreeItem<double?>>(root);

        tree.ForEach(i => i.Value += 1);

        tree.ForEachAsync(i => i.Value += 1);

        tree.ForEachParallelAsync(i => i.Value += 1);

        tree.ForEach(i => Console.WriteLine(i));
    }

    [Test]
    public void TreeMapTest()
    {
        var root = new TreeItem<string>("root")
        {
            Children =
            {
                new("child1") { Children = { new("child11"), new("child12"), new("child13") } },
                new("child2") { Children = { new("child21"), new("child22"), new("child23") } },
                new("child3") { Children = { new("child31"), new("child32"), new("child33") } },
            },
        };

        var tree = root.GetTree();

        var result = tree.TreeMap(item => new TreeItem<string>(item.Value + "*"));

        foreach (var (original, mapped) in tree.Zip(result, (first, second) => (first, second)))
        {
            Assert.That(original.Value + "*", Is.EqualTo(mapped.Value));
        }
    }
}
