using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;
using RefraSin.Graphs;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Nodes.Extensions;
using RefraSin.ParticleModel.Particles;
using static NUnit.Framework.Assert;
using static RefraSin.Coordinates.Constants;

namespace RefraSin.ParticleModel.Test;

[TestFixture]
public class GeometryTest
{
    record DummyNode(
        Guid Id,
        Guid ParticleId,
        IPolarPoint Coordinates,
        NodeType Type,
        INode Upper,
        INode Lower
    ) : INode, INodeGeometry, INodeNeighbors
    {
        public ToUpperToLower<double> SurfaceDistance => this.SurfaceDistance();
        public ToUpperToLower<Angle> SurfaceRadiusAngle => this.SurfaceRadiusAngle();
        public ToUpperToLower<Angle> AngleDistance => this.AngleDistance();
        public ToUpperToLower<double> Volume => this.Volume();
        public ToUpperToLower<Angle> SurfaceNormalAngle => this.SurfaceNormalAngle();
        public ToUpperToLower<Angle> SurfaceTangentAngle => this.SurfaceTangentAngle();
    }

    [Test]
    public void TestAngleDistance()
    {
        var upper = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(HalfOfPi, 0.5),
            NodeType.Surface
        );
        var lower = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(-ThirdOfPi, 1),
            NodeType.Surface
        );
        var node = new DummyNode(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(QuarterOfPi, 1),
            NodeType.Surface,
            upper,
            lower
        );

        That((double)node.AngleDistance.ToUpper, Is.EqualTo(QuarterOfPi).Within(1e-8));
        That((double)node.AngleDistance.ToLower, Is.EqualTo(QuarterOfPi + ThirdOfPi).Within(1e-8));
    }

    [Test]
    public void TestSurfaceDistance()
    {
        var upper = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(HalfOfPi, 0.5),
            NodeType.Surface
        );
        var lower = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(-ThirdOfPi, 1),
            NodeType.Surface
        );
        var node = new DummyNode(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(QuarterOfPi, 1),
            NodeType.Surface,
            upper,
            lower
        );

        That(node.SurfaceDistance.ToUpper, Is.EqualTo(0.7368).Within(1e-4));
        That(node.SurfaceDistance.ToLower, Is.EqualTo(1.5867).Within(1e-4));
    }

    [Test]
    public void TestSurfaceRadiusAngle()
    {
        var upper = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(HalfOfPi, 0.5),
            NodeType.Surface
        );
        var lower = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(-ThirdOfPi, 1),
            NodeType.Surface
        );
        var node = new DummyNode(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(QuarterOfPi, 1),
            NodeType.Surface,
            upper,
            lower
        );

        That((double)node.SurfaceRadiusAngle.ToUpper, Is.EqualTo(0.50047).Within(1e-4));
        That(
            (double)node.SurfaceRadiusAngle.ToLower,
            Is.EqualTo((Pi - QuarterOfPi - ThirdOfPi) / 2).Within(1e-8)
        );
    }

    [Test]
    public void TestVolume()
    {
        var upper = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(HalfOfPi, 0.5),
            NodeType.Surface
        );
        var lower = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(-ThirdOfPi, 1),
            NodeType.Surface
        );
        var node = new DummyNode(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(QuarterOfPi, 1),
            NodeType.Surface,
            upper,
            lower
        );

        That(node.Volume.ToUpper, Is.EqualTo(0.17677).Within(1e-4));
        That(node.Volume.ToLower, Is.EqualTo(0.48296).Within(1e-4));
    }

    [Test]
    public void TestSurfaceNormalAngle()
    {
        var upper = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(HalfOfPi, 0.5),
            NodeType.Surface
        );
        var lower = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(-ThirdOfPi, 1),
            NodeType.Surface
        );
        var node = new DummyNode(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(QuarterOfPi, 1),
            NodeType.Surface,
            upper,
            lower
        );

        That((double)node.SurfaceNormalAngle.ToUpper, Is.EqualTo(2.564106).Within(1e-4));
        That(
            node.SurfaceNormalAngle.ToLower,
            Is.EqualTo(node.SurfaceNormalAngle.ToUpper).Within(1e-8)
        );
    }

    [Test]
    public void TestSurfaceNormalAngleNeckLowerIsGrainBoundary()
    {
        var upper = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(HalfOfPi, 0.5),
            NodeType.Surface
        );
        var lower = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(-ThirdOfPi, 1),
            NodeType.GrainBoundary
        );
        var node = new DummyNode(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(QuarterOfPi, 1),
            NodeType.Neck,
            upper,
            lower
        );

        That(
            (double)node.SurfaceNormalAngle.ToUpper,
            Is.EqualTo(2.564106 * 2 - HalfOfPi).Within(1e-4)
        );
        That((double)node.SurfaceNormalAngle.ToLower, Is.EqualTo(HalfOfPi).Within(1e-8));
    }

    [Test]
    public void TestSurfaceNormalAngleNeckUpperIsGrainBoundary()
    {
        var upper = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(HalfOfPi, 0.5),
            NodeType.GrainBoundary
        );
        var lower = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(-ThirdOfPi, 1),
            NodeType.Surface
        );
        var node = new DummyNode(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(QuarterOfPi, 1),
            NodeType.Neck,
            upper,
            lower
        );

        That(
            (double)node.SurfaceNormalAngle.ToLower,
            Is.EqualTo(2.564106 * 2 - HalfOfPi).Within(1e-4)
        );
        That((double)node.SurfaceNormalAngle.ToUpper, Is.EqualTo(HalfOfPi).Within(1e-8));
    }

    [Test]
    public void TestSurfaceTangentAngleNeckLowerIsGrainBoundary()
    {
        var upper = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(HalfOfPi, 0.5),
            NodeType.Surface
        );
        var lower = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(-ThirdOfPi, 1),
            NodeType.GrainBoundary
        );
        var node = new DummyNode(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(QuarterOfPi, 1),
            NodeType.Neck,
            upper,
            lower
        );

        That((double)node.SurfaceTangentAngle.ToUpper, Is.EqualTo(2.564106 * 2 - Pi).Within(1e-4));
        That((double)node.SurfaceTangentAngle.ToLower, Is.EqualTo(0).Within(1e-8));
    }

    [Test]
    public void TestSurfaceTangentAngleNeckUpperIsGrainBoundary()
    {
        var upper = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(HalfOfPi, 0.5),
            NodeType.GrainBoundary
        );
        var lower = new Node(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(-ThirdOfPi, 1),
            NodeType.Surface
        );
        var node = new DummyNode(
            Guid.NewGuid(),
            Guid.Empty,
            new PolarPoint(QuarterOfPi, 1),
            NodeType.Neck,
            upper,
            lower
        );

        That((double)node.SurfaceTangentAngle.ToLower, Is.EqualTo(2.564106 * 2 - Pi).Within(1e-4));
        That((double)node.SurfaceTangentAngle.ToUpper, Is.EqualTo(0).Within(1e-8));
    }
}
