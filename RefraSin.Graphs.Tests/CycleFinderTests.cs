using MoreLinq.Extensions;

namespace RefraSin.Graphs.Tests;

public class CycleFinderTests
{
    [Test]
    public void TestFindCycle()
    {
        var vertices = Enumerable
            .Range(0, 14)
            .Select(i => new Vertex(Guid.NewGuid(), $"V{i}"))
            .ToArray();

        var graph = new DirectedGraph<Vertex, Edge<Vertex>>(
            vertices,
            new[]
            {
                new Edge<Vertex>(vertices[0], vertices[1]),
                new Edge<Vertex>(vertices[0], vertices[2]),
                new Edge<Vertex>(vertices[0], vertices[3]),
                new Edge<Vertex>(vertices[1], vertices[4]),
                new Edge<Vertex>(vertices[1], vertices[5]),
                new Edge<Vertex>(vertices[2], vertices[6]),
                new Edge<Vertex>(vertices[2], vertices[7]),
                new Edge<Vertex>(vertices[3], vertices[8]),
                new Edge<Vertex>(vertices[3], vertices[9]),
                new Edge<Vertex>(vertices[4], vertices[10]),
                new Edge<Vertex>(vertices[6], vertices[12]),
                new Edge<Vertex>(vertices[8], vertices[13]),
                // cycles
                new Edge<Vertex>(vertices[3], vertices[4]),
                new Edge<Vertex>(vertices[2], vertices[1]),
                new Edge<Vertex>(vertices[5], vertices[12]),
                new Edge<Vertex>(vertices[7], vertices[13]),
                new Edge<Vertex>(vertices[9], vertices[13]),
            }
        );

        var cycles = CycleFinder<Vertex, Edge<Vertex>>
            .FindCycles(graph, vertices[0])
            .FoundCycles.ToArray();

        Assert.That(cycles, Has.Length.EqualTo(5));
        foreach (var c in cycles)
        {
            var firstPath = c.FirstPath.ToArray();
            var secondPath = c.SecondPath.ToArray();
            Assert.That(firstPath, Has.Length.GreaterThanOrEqualTo(1));
            Assert.That(secondPath, Has.Length.GreaterThanOrEqualTo(1));

            Assert.That(firstPath[0].From, Is.EqualTo(secondPath[0].From));
            Assert.That(firstPath[^1].To, Is.EqualTo(secondPath[^1].To));
        }
    }
}
