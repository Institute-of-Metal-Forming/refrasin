using NUnit.Framework;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Helpers;

namespace RefraSin.Coordinates.Test;

[TestFixture]
public class LineTest
{
    public static IEnumerable<TestCaseData> GenerateIntersectionData()
    {
        yield return new TestCaseData(
            new AbsolutePoint(0, 0),
            new AbsoluteVector(1, 1),
            new AbsolutePoint(2, 0),
            new AbsoluteVector(-1, 1),
            new AbsolutePoint(1, 1)
        );

        yield return new TestCaseData(
            new AbsolutePoint(0, 0),
            new AbsoluteVector(1, 0),
            new AbsolutePoint(2, 0),
            new AbsoluteVector(0, 1),
            new AbsolutePoint(2, 0)
        );

        yield return new TestCaseData(
            new AbsolutePoint(1, 1),
            new AbsoluteVector(-1, -1),
            new AbsolutePoint(2, 0),
            new AbsoluteVector(-1, 0),
            new AbsolutePoint(0, 0)
        );
    }

    [Test]
    [TestCaseSource(nameof(GenerateIntersectionData))]
    public void TestIntersection(IPoint p1, IVector v1, IPoint p2, IVector v2, IPoint result)
    {
        var line1 = new AbsoluteLine(p1, v1);
        var line2 = new AbsoluteLine(p2, v2);
        var sol = line1.IntersectionTo(line2);
        Assert.That(sol.X, Is.EqualTo(result.Absolute.X).Within(1e-8));
        Assert.That(sol.Y, Is.EqualTo(result.Absolute.Y).Within(1e-8));
    }
}
