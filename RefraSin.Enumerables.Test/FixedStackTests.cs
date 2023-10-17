using NUnit.Framework;

namespace RefraSin.Enumerables.Test;

[TestFixture]
public class FixedStackTests
{
    private FixedStack<int> _stack = new(3);

    [SetUp]
    public void SetUp()
    {
        _stack.Push(1);
        _stack.Push(2);
        _stack.Push(3);
    }

    [Test]
    public void TestPush()
    {
        _stack.Push(4);
        Assert.That(_stack.Tail, Is.Not.EqualTo(1));
        Assert.That(_stack.Tail, Is.EqualTo(2));
    }

    [Test]
    public void TestPop()
    {
        var item = _stack.Pop();
        Assert.That(item, Is.EqualTo(3));
    }

    [Test]
    public void TestDoublePop()
    {
        _stack.Pop();
        var item = _stack.Pop();
        Assert.That(item, Is.EqualTo(2));
    }

    [Test]
    public void TestPopPushPop()
    {
        var item = _stack.Pop();
        Assert.That(item, Is.EqualTo(3));

        _stack.Push(4);

        item = _stack.Pop();
        Assert.That(item, Is.EqualTo(4));

        Assert.That(_stack.Tail, Is.EqualTo(1));
    }
}