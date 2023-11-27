namespace RefraSin.Graphs.Tests;

[TestFixture]
public class VertexTests
{
    private Vertex _vertex1;
    private Vertex _vertex2;
    private Vertex _vertex3;

    [SetUp]
    public void Setup()
    {
        _vertex1 = new Vertex(Guid.NewGuid());
        _vertex2 = new Vertex(Guid.NewGuid());
        _vertex3 = new Vertex(_vertex1.Id);
    }

    [Test]
    public void TestEquality()
    {
        Assert.That(
            _vertex1,
            Is.EqualTo(
                _vertex3
            )
        );
        Assert.That(
            _vertex3,
            Is.EqualTo(
                _vertex1
            )
        );
        Assert.That(
            _vertex1,
            Is.Not.EqualTo(
                _vertex2
            )
        );
    }
}