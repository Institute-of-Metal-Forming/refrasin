using System.Globalization;
using NUnit.Framework;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Cartesian;
using RefraSin.Coordinates.Polar;
using static System.Math;
using static RefraSin.Coordinates.Angle.ReductionDomain;
using static MathNet.Numerics.Constants;

namespace RefraSin.Coordinates.Test;

[TestFixture]
public class CoordinatesTest
{
    [Test]
    public void CartesianToPolarTest()
    {
        Assert.True(new PolarPoint(new AbsolutePoint(1, 0)).Equals(new PolarPoint(0, 1)));
        Assert.True(
            new PolarPoint(new AbsolutePoint(1, 1)).Equals(new PolarPoint(PI / 4, Sqrt2)));
        Assert.True(new PolarPoint(new AbsolutePoint(0, 1)).Equals(new PolarPoint(PI / 2, 1)));
        Assert.True(
            new PolarPoint(new AbsolutePoint(-1, 1)).Equals(
                new PolarPoint(3 * PI / 4, Sqrt2)));
        Assert.True(new PolarPoint(new AbsolutePoint(-1, 0)).Equals(new PolarPoint(PI, 1)));
        Assert.True(new PolarPoint(new AbsolutePoint(0, 0)).Equals(new PolarPoint(0, 0)));
        Assert.True(
            new PolarPoint(new AbsolutePoint(0, -1)).Equals(
                new PolarPoint(3 * PI / 2, 1)));
        Assert.True(
            new PolarPoint(new AbsolutePoint(-1, -1)).Equals(
                new PolarPoint(5 * PI / 4, Sqrt2)));
    }

    [Test]
    public void AngleToTest()
    {
        var p1 = new PolarPoint(0.4 * PI, 1);

        // non reduced
        Assert.AreEqual(0.1 * PI, 0.5 * PI - p1.Phi, 1e-3);
        Assert.AreEqual(0.8 * PI, 1.2 * PI - p1.Phi, 1e-3);
        Assert.AreEqual(1.1 * PI, 1.5 * PI - p1.Phi, 1e-3);
        Assert.AreEqual(-0.1 * PI, 0.3 * PI - p1.Phi, 1e-3);
        Assert.AreEqual(-0.5 * PI, -0.1 * PI - p1.Phi, 1e-3);

        // reduced to [0, 2π]
        Assert.AreEqual(0.1 * PI, (0.5 * PI - p1.Phi).Reduce(), 1e-3);
        Assert.AreEqual(0.8 * PI, (1.2 * PI - p1.Phi).Reduce(), 1e-3);
        Assert.AreEqual(1.1 * PI, (1.5 * PI - p1.Phi).Reduce(), 1e-3);
        Assert.AreEqual(1.9 * PI, (0.3 * PI - p1.Phi).Reduce(), 1e-3);
        Assert.AreEqual(1.5 * PI, (-0.1 * PI - p1.Phi).Reduce(), 1e-3);

        // reduced to [-π, π]
        Assert.AreEqual(0.1 * PI, (0.5 * PI - p1.Phi).Reduce(WithNegative), 1e-3);
        Assert.AreEqual(0.8 * PI, (1.2 * PI - p1.Phi).Reduce(WithNegative), 1e-3);
        Assert.AreEqual(-0.9 * PI, (1.5 * PI - p1.Phi).Reduce(WithNegative), 1e-3);
        Assert.AreEqual(-0.1 * PI, (0.3 * PI - p1.Phi).Reduce(WithNegative), 1e-3);
        Assert.AreEqual(-0.5 * PI, (-0.1 * PI - p1.Phi).Reduce(WithNegative), 1e-3);

        Assert.AreEqual(0.1 * PI, p1.AngleTo(new PolarPoint(0.5 * PI, 2)), 1e-3);
        Assert.Throws<DifferentCoordinateSystemException>(() =>
            p1.AngleTo(new PolarPoint(0, 1, new PolarCoordinateSystem(null, 1))));
    }

    [Test]
    public void CartesianTransformationTest()
    {
        var p1 = new CartesianPoint(1, 2);
        var p2 = new CartesianPoint(-1, -2);

        p1.RotateBy(PI);

        Assert.True(p1.Equals(p2));
    }

    [Test]
    public void CartesianConversionTest()
    {
        var p = new AbsolutePoint(1, 1);

        var sys1 = new CartesianCoordinateSystem(new AbsolutePoint(1, 0), 0.25 * PI, 2);
        var p1 = new CartesianPoint(p, sys1);
        Assert.True(p1.Equals(new CartesianPoint(0.5 / Sqrt2, 1 / Sqrt2, sys1)));

        var sys2 = new PolarCoordinateSystem(new AbsolutePoint(1, 0), 0.25 * PI, 2);
        var p2 = new PolarPoint(p, sys2);
        Assert.True(p2.Equals(new PolarPoint(0.25 * PI, 0.5, sys2)));

        Assert.True(p.Equals(new CartesianPoint(p2, sys1).Absolute));
    }

    [Test]
    public void PolarPolarTest()
    {
        var p1 = new PolarPoint(PI / 4, 1);
        Assert.True(p1.Absolute.Equals(new AbsolutePoint(Sqrt2 / 2, Sqrt2 / 2)));

        var sys1 = new PolarCoordinateSystem(p1, PI / 4, 2);
        var p2 = new PolarPoint(0, 1, sys1);
        Assert.True(p2.Absolute.Equals(new AbsolutePoint(3 * Sqrt2 / 2, 3 * Sqrt2 / 2)));
    }

    [Test]
    public void DistanceTest()
    {
        var p1 = new PolarPoint(0, 1);
        var p2 = new PolarPoint(PI / 2, 1);
        var p3 = new PolarPoint(0, 1, new PolarCoordinateSystem(new AbsolutePoint(1, 1), PI));

        Assert.AreEqual(Sqrt2, p1.DistanceTo(p2), 1e-3);
        Assert.AreEqual(Sqrt2, p1.Absolute.DistanceTo(p2.Absolute), 1e-3);
        Assert.AreEqual(Sqrt2, p1.Absolute.DistanceTo(p3.Absolute), 1e-3);

        var c1 = new CartesianPoint(1, 0);
        var c2 = new CartesianPoint(0, 1);

        Assert.AreEqual(Sqrt2, c1.DistanceTo(c2), 1e-3);
        Assert.AreEqual(Sqrt2, c1.Absolute.DistanceTo(c2.Absolute), 1e-3);
        Assert.AreEqual(Sqrt2, c1.Absolute.DistanceTo(p3.Absolute), 1e-3);
    }

    [Test]
    public void HalfWayTest()
    {
        var p1 = new PolarPoint(0.1, 1);
        var p2 = new PolarPoint(0.3, 2);
        var p3 = new PolarPoint(-0.1, 0.5);
        var p4 = new PolarPoint(Pi2 + Pi + 0.3, 2);

        var hw12 = p1.Absolute.PointHalfWayTo(p2.Absolute);
        var hw13 = p1.Absolute.PointHalfWayTo(p3.Absolute);
        var hw14 = p1.Absolute.PointHalfWayTo(p4.Absolute);

        Assert.AreEqual(hw12, p1.PointHalfWayTo(p2).Absolute);
        Assert.AreEqual(hw13, p1.PointHalfWayTo(p3).Absolute);
        Assert.AreEqual(hw13, p3.PointHalfWayTo(p1).Absolute);
        Assert.AreEqual(hw14, p1.PointHalfWayTo(p4).Absolute);
    }

    [Test]
    public void AngleParseTest()
    {
        foreach (var a in new Angle[] {Pi3Over2, Pi3Over2, Pi2, 3 * Pi, -3 * Pi})
        {
            foreach (var f in new[] {":rad", ":deg", ":gon", ":grad"})
            {
                var s1 = a.ToString(f);
                Console.WriteLine(s1);
                Assert.AreEqual(a, Angle.Parse(s1));

                var s2 = a.ToString(f + "+");
                Console.WriteLine(s2);
                Assert.AreEqual(a.Reduce(AllPositive), Angle.Parse(s2));

                var s3 = a.ToString(f + "-");
                Console.WriteLine(s3);
                Assert.AreEqual(a.Reduce(WithNegative), Angle.Parse(s3));
            }
        }
    }

    [Test]
    public void AngleReductionTest()
    {
        for (var i = 0; i <= 50; i++)
        {
            {
                var a = new Angle(i * Pi / 10).Reduce(AllPositive);
                Console.WriteLine(a.ToString());
                Assert.IsTrue(a.IsInDomain(AllPositive));
                Assert.IsTrue(a.Radians is <= Pi2 and >= 0, "{0} < Pi && {0} > -Pi", a);
            }

            {
                var a = new Angle(i * Pi / 10).Reduce(WithNegative);
                Console.WriteLine(a.ToString());
                Assert.IsTrue(a.IsInDomain(WithNegative));
                Assert.IsTrue(a.Radians is <= Pi and >= -Pi, "{0} < Pi && {0} > -Pi", a);
            }
        }
    }

    [Test]
    public void AngleEqualityTest()
    {
        Assert.IsFalse(new Angle(Pi).AlmostEqual(new Angle(-Pi)));
        Assert.IsTrue(new Angle(Pi, AllPositive).AlmostEqual(new Angle(-Pi, AllPositive)));
        Assert.IsTrue(new Angle(Pi, WithNegative).AlmostEqual(new Angle(-Pi, WithNegative)));
    }

    [Test]
    public void AngleGreaterSmallerTest()
    {
        foreach (var i in Enumerable.Range(0, 6))
        {
            var a = new Angle(i);
            var a1 = a + PiOver2;
            var a2 = a - PiOver2;
            var a3 = a + Pi;
            var a4 = a + 0.1;
            var a5 = a - 0.1;
            var a6 = a - 0.9 * Pi;

            Assert.IsTrue(a1 > a);
            Assert.IsTrue(a2 < a);
            Assert.IsTrue(a4 > a);
            Assert.IsTrue(a5 < a);
            Assert.IsTrue(a6 < a);

            Assert.IsFalse(a1 < a);
            Assert.IsFalse(a2 > a);
            Assert.IsFalse(a4 < a);
            Assert.IsFalse(a5 > a);
            Assert.IsFalse(a6 > a);

            // Assert.IsFalse(a3 > a);
            Assert.IsFalse(a3 < a);
        }
    }

    [Test]
    public void VectorAdditionTest()
    {
        var ap1 = new AbsolutePoint(1, 1);
        var av1 = new AbsoluteVector(0, 1);
        Assert.AreEqual(new AbsolutePoint(1, 2), ap1 + av1);

        var cp1 = new AbsolutePoint(1, 1);
        var cv1 = new AbsoluteVector(0, 1);
        Assert.AreEqual(new AbsolutePoint(1, 2), cp1 + cv1);

        var pp1 = new PolarPoint(PiOver4, 1);
        var pp2 = new PolarPoint(0, 1);
        var pv1 = new PolarVector(0, 1);
        var pv2 = new PolarVector(3 * PiOver4, 1);
        Assert.AreEqual(new AbsolutePoint(1 + Sqrt2 / 2, Sqrt2 / 2), (pp1 + pv1).Absolute);
        Assert.AreEqual(new AbsolutePoint(0, Sqrt2), (pp1 + pv2).Absolute);
        Assert.AreEqual(new AbsolutePoint(1, Sqrt2), (pp1 + pv1 + pv2).Absolute);
        Assert.AreEqual(new AbsolutePoint(1, Sqrt2), (pp1 + (pv1 + pv2)).Absolute);

        Assert.AreEqual(new AbsolutePoint(2, 0), (pp2 + pv1).Absolute);
        Assert.AreEqual(new AbsolutePoint(0, 0), (pp2 + -pv1).Absolute);

        Assert.AreEqual(pv1 + pv2, pp1 + pv1 + pv2 - pp1);
        Assert.AreEqual(pv1 + pv2, pp1 + (pv1 + pv2) - pp1);
    }

    [Test]
    public void CoordinatesToStringAndParseTest()
    {
        var pp = new PolarPoint(PI, 1.2);
        var pv = new PolarVector(PI, 1.2);
        var cp = new CartesianPoint(1.3, 1.2);
        var cv = new CartesianVector(1.3, 1.2);
        var ap = new AbsolutePoint(1.3, 1.2);
        var av = new AbsoluteVector(1.3, 1.2);

        foreach (var format in new[] {":e3:deg", "V[]:f2:rad", "<;>::grad", "N{V,}:f2:gon"})
        {
            var s = pp.ToString(format, null);
            Console.WriteLine(s);
            Assert.IsTrue(PolarPoint.Parse(s).Equals(pp, 2));

            s = pv.ToString(format, null);
            Console.WriteLine(s);
            Assert.IsTrue(PolarVector.Parse(s).Equals(pv, 2));

            s = cp.ToString(format, null);
            Console.WriteLine(s);
            Assert.IsTrue(CartesianPoint.Parse(s).Equals(cp, 2));

            s = cv.ToString(format, null);
            Console.WriteLine(s);
            Assert.IsTrue(CartesianVector.Parse(s).Equals(cv, 2));

            s = ap.ToString(format, null);
            Console.WriteLine(s);
            Assert.IsTrue(AbsolutePoint.Parse(s).Equals(ap, 2));

            s = av.ToString(format, null);
            Console.WriteLine(s);
            Assert.IsTrue(AbsoluteVector.Parse(s).Equals(av, 2));
        }
    }

    [Test]
    public void VectorEnumerableTest()
    {
        var vectors = new[]
        {
            new PolarVector(0, 1),
            new PolarVector(PiOver2, 1),
            new PolarVector(Pi, 1),
            new PolarVector(Pi3Over2, 1)
        };

        Assert.AreEqual(new PolarVector(PiOver2, 0), vectors.Sum());

        var vectors2 = new[]
        {
            new PolarVector(0, Sqrt2),
            new PolarVector(PiOver2, Sqrt2),
            new PolarVector(PiOver4, 1)
        };

        Assert.AreEqual(new PolarVector(PiOver4, 1), vectors2.Average());
        Assert.AreEqual(new PolarVector(PiOver4, 1), vectors2.AverageDirection());
    }

    class OriginClass
    {
        public IPoint Point { get; set; }
    }

    [Test]
    public void OriginSourceTest()
    {
        var o1 = new AbsolutePoint(1, 1);
        var o2 = new AbsolutePoint(-1, 2);

        var system = new CartesianCoordinateSystem();

        var p1 = new CartesianPoint(1.0, 1.0, system);

        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        Assert.AreEqual(new AbsolutePoint(1, 1), p1.Absolute);

        system.Origin = o1;
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        Assert.AreEqual(new AbsolutePoint(2, 2), p1.Absolute);

        o1 = new AbsolutePoint(0, 0);
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        Assert.AreNotEqual(new AbsolutePoint(1, 1), p1.Absolute);

        // ReSharper disable once AccessToModifiedClosure
        system.OriginSource = () => o2;
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        Assert.AreEqual(new AbsolutePoint(0, 3), p1.Absolute);

        o2 = new AbsolutePoint(0, 0);
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        Assert.AreEqual(new AbsolutePoint(1, 1), p1.Absolute);

        var instance = new OriginClass()
        {
            Point = new AbsolutePoint(-1, -1)
        };
        system.OriginSource = () => instance.Point;
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        Assert.AreEqual(new AbsolutePoint(0, 0), p1.Absolute);

        instance.Point = new AbsolutePoint(0, 0);
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        Assert.AreEqual(new AbsolutePoint(1, 1), p1.Absolute);
    }

    [Test]
    public void PolarCoordinates_AngleReduction()
    {
        var s1 = new PolarCoordinateSystem();
        var p1 = new PolarPoint(3 * Pi, 1, s1);
        Console.WriteLine($"p1:\t{p1}");
        Assert.IsFalse(p1.Phi.IsInDomain(AllPositive));
        Assert.IsFalse(p1.Phi.IsInDomain(WithNegative));
        Assert.AreEqual(3 * Pi, p1.Phi.Radians);

        var s2 = new PolarCoordinateSystem(angleReductionDomain: AllPositive);
        var p21 = new PolarPoint(3 * Pi, 1, s2);
        Console.WriteLine($"p21:\t{p21}");
        Assert.IsTrue(p21.Phi.IsInDomain(AllPositive));
        Assert.IsFalse(p21.Phi.IsInDomain(WithNegative));
        Assert.AreEqual(Pi, p21.Phi.Radians);
        var p22 = new PolarPoint(Pi2, 1, s2);
        Console.WriteLine($"p22:\t{p22}");
        Assert.IsTrue(p22.Phi.IsInDomain(AllPositive));
        Assert.IsTrue(p22.Phi.IsInDomain(WithNegative));
        Assert.AreEqual(0, p22.Phi.Radians);

        var s3 = new PolarCoordinateSystem(angleReductionDomain: WithNegative);
        var p31 = new PolarPoint(1.5 * Pi, 1, s3);
        Console.WriteLine($"p31:\t{p31}");
        Assert.IsFalse(p31.Phi.IsInDomain(AllPositive));
        Assert.IsTrue(p31.Phi.IsInDomain(WithNegative));
        Assert.AreEqual(-0.5 * Pi, p31.Phi.Radians);
        var p32 = new PolarPoint(Pi, 1, s3);
        Console.WriteLine($"p32:\t{p32}");
        Assert.IsFalse(p32.Phi.IsInDomain(AllPositive));
        Assert.IsTrue(p32.Phi.IsInDomain(WithNegative));
        Assert.AreEqual(-Pi, p32.Phi.Radians);

        s3.AngleReductionDomain = AllPositive;
        Console.WriteLine($"p31:\t{p31}");
        Assert.IsTrue(p31.Phi.IsInDomain(AllPositive));
        Assert.IsFalse(p31.Phi.IsInDomain(WithNegative));
        Assert.AreEqual(1.5 * Pi, p31.Phi.Radians);
        Console.WriteLine($"p32:\t{p32}");
        Assert.IsTrue(p32.Phi.IsInDomain(AllPositive));
        Assert.IsFalse(p32.Phi.IsInDomain(WithNegative));
        Assert.AreEqual(Pi, p32.Phi.Radians);
    }
}