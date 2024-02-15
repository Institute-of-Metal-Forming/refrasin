using System.Globalization;
using NUnit.Framework;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Cartesian;
using RefraSin.Coordinates.Polar;
using static System.Math;
using static RefraSin.Coordinates.Angle.ReductionDomain;
using static RefraSin.Coordinates.Constants;
using static NUnit.Framework.Assert;
using static NUnit.Framework.Legacy.ClassicAssert;

namespace RefraSin.Coordinates.Test;

[TestFixture]
public class CoordinatesTest
{
    [Test]
    public void CartesianToPolarTest()
    {
        That(new PolarPoint(new AbsolutePoint(1, 0)).IsClose(new PolarPoint(0, 1)));
        That(
            new PolarPoint(new AbsolutePoint(1, 1)).IsClose(new PolarPoint(PI / 4, Sqrt2)));
        That(new PolarPoint(new AbsolutePoint(0, 1)).IsClose(new PolarPoint(PI / 2, 1)));
        That(
            new PolarPoint(new AbsolutePoint(-1, 1)).IsClose(
                new PolarPoint(3 * PI / 4, Sqrt2)));
        That(new PolarPoint(new AbsolutePoint(-1, 0)).IsClose(new PolarPoint(PI, 1)));
        That(new PolarPoint(new AbsolutePoint(0, 0)).IsClose(new PolarPoint(0, 0)));
        That(
            new PolarPoint(new AbsolutePoint(0, -1)).IsClose(
                new PolarPoint(3 * PI / 2, 1)));
        That(
            new PolarPoint(new AbsolutePoint(-1, -1)).IsClose(
                new PolarPoint(5 * PI / 4, Sqrt2)));
    }

    [Test]
    public void AngleToTest()
    {
        var p1 = new PolarPoint(0.4 * PI, 1);

        // non reduced
        That(0.5 * PI - p1.Phi, Is.EqualTo(new Angle(0.1 * PI)).Within(1e-3));
        That(1.2 * PI - p1.Phi, Is.EqualTo(new Angle(0.8 * PI)).Within(1e-3));
        That(1.5 * PI - p1.Phi, Is.EqualTo(new Angle(1.1 * PI)).Within(1e-3));
        That(0.3 * PI - p1.Phi, Is.EqualTo(new Angle(-0.1 * PI)).Within(1e-3));
        That(-0.1 * PI - p1.Phi, Is.EqualTo(new Angle(-0.5 * PI)).Within(1e-3));

        // reduced to [0, 2π]
        That((0.5 * PI - p1.Phi).Reduce(), Is.EqualTo(new Angle(0.1 * PI)).Within(1e-3));
        That((1.2 * PI - p1.Phi).Reduce(), Is.EqualTo(new Angle(0.8 * PI)).Within(1e-3));
        That((1.5 * PI - p1.Phi).Reduce(), Is.EqualTo(new Angle(1.1 * PI)).Within(1e-3));
        That((0.3 * PI - p1.Phi).Reduce(), Is.EqualTo(new Angle(1.9 * PI)).Within(1e-3));
        That((-0.1 * PI - p1.Phi).Reduce(), Is.EqualTo(new Angle(1.5 * PI)).Within(1e-3));

        // reduced to [-π, π]
        That((0.5 * PI - p1.Phi).Reduce(WithNegative), Is.EqualTo(new Angle(0.1 * PI)).Within(1e-3));
        That((1.2 * PI - p1.Phi).Reduce(WithNegative), Is.EqualTo(new Angle(0.8 * PI)).Within(1e-3));
        That((1.5 * PI - p1.Phi).Reduce(WithNegative), Is.EqualTo(new Angle(-0.9 * PI)).Within(1e-3));
        That((0.3 * PI - p1.Phi).Reduce(WithNegative), Is.EqualTo(new Angle(-0.1 * PI)).Within(1e-3));
        That((-0.1 * PI - p1.Phi).Reduce(WithNegative), Is.EqualTo(new Angle(-0.5 * PI)).Within(1e-3));

        That(p1.AngleTo(new PolarPoint(0.5 * PI, 2)), Is.EqualTo(new Angle(0.1 * PI)).Within(1e-3));
        Throws<DifferentCoordinateSystemException>(() =>
            p1.AngleTo(new PolarPoint(0, 1, new PolarCoordinateSystem(null, 1))));
    }

    [Test]
    public void CartesianTransformationTest()
    {
        var p1 = new CartesianPoint(1, 2);
        var p2 = new CartesianPoint(-1, -2);

        p1.RotateBy(PI);

        That(p1.IsClose(p2));
    }

    [Test]
    public void CartesianConversionTest()
    {
        var p = new AbsolutePoint(1, 1);

        var sys1 = new CartesianCoordinateSystem(new AbsolutePoint(1, 0), 0.25 * PI, 2);
        var p1 = new CartesianPoint(p, sys1);
        That(p1.IsClose(new CartesianPoint(0.5 / Sqrt2, 1 / Sqrt2, sys1)));

        var sys2 = new PolarCoordinateSystem(new AbsolutePoint(1, 0), 0.25 * PI, 2);
        var p2 = new PolarPoint(p, sys2);
        That(p2.IsClose(new PolarPoint(0.25 * PI, 0.5, sys2)));

        That(p.IsClose(new CartesianPoint(p2, sys1).Absolute));
    }

    [Test]
    public void PolarPolarTest()
    {
        var p1 = new PolarPoint(PI / 4, 1);
        That(p1.Absolute.IsClose(new AbsolutePoint(Sqrt2 / 2, Sqrt2 / 2)));

        var sys1 = new PolarCoordinateSystem(p1, PI / 4, 2);
        var p2 = new PolarPoint(0, 1, sys1);
        That(p2.Absolute.IsClose(new AbsolutePoint(3 * Sqrt2 / 2, 3 * Sqrt2 / 2)));
    }

    [Test]
    public void DistanceTest()
    {
        var p1 = new PolarPoint(0, 1);
        var p2 = new PolarPoint(PI / 2, 1);
        var p3 = new PolarPoint(0, 1, new PolarCoordinateSystem(new AbsolutePoint(1, 1), PI));

        That(p1.DistanceTo(p2), Is.EqualTo(Sqrt2).Within(1e-3));
        That(p1.Absolute.DistanceTo(p2.Absolute), Is.EqualTo(Sqrt2).Within(1e-3));
        That(p1.Absolute.DistanceTo(p3.Absolute), Is.EqualTo(Sqrt2).Within(1e-3));

        var c1 = new CartesianPoint(1, 0);
        var c2 = new CartesianPoint(0, 1);

        That(c1.DistanceTo(c2), Is.EqualTo(Sqrt2).Within(1e-3));
        That(c1.Absolute.DistanceTo(c2.Absolute), Is.EqualTo(Sqrt2).Within(1e-3));
        That(c1.Absolute.DistanceTo(p3.Absolute), Is.EqualTo(Sqrt2).Within(1e-3));
    }

    [Test]
    public void HalfWayTest()
    {
        var p1 = new PolarPoint(0.1, 1);
        var p2 = new PolarPoint(0.3, 2);
        var p3 = new PolarPoint(-0.1, 0.5);
        var p4 = new PolarPoint(TwoPi + Pi + 0.3, 2);

        var hw12 = p1.Absolute.PointHalfWayTo(p2.Absolute);
        var hw13 = p1.Absolute.PointHalfWayTo(p3.Absolute);
        var hw14 = p1.Absolute.PointHalfWayTo(p4.Absolute);

        That(p1.PointHalfWayTo(p2).Absolute.IsClose(hw12));
        That(p1.PointHalfWayTo(p3).Absolute.IsClose(hw13));
        That(p3.PointHalfWayTo(p1).Absolute.IsClose(hw13));
        That(p1.PointHalfWayTo(p4).Absolute.IsClose(hw14));
    }

    [Test]
    public void AngleParseTest()
    {
        foreach (var a in new Angle[] { ThreeHalfsOfPi, ThreeHalfsOfPi, TwoPi, 3 * Pi, -3 * Pi })
        {
            foreach (var f in new[] { ":rad", ":deg", ":gon", ":grad" })
            {
                var s1 = a.ToString(f);
                Console.WriteLine(s1);
                That(Angle.Parse(s1), Is.EqualTo(a).Within(1e-8));

                var s2 = a.ToString(f + "+");
                Console.WriteLine(s2);
                That(Angle.Parse(s2), Is.EqualTo(a.Reduce(AllPositive)).Within(1e-8));

                var s3 = a.ToString(f + "-");
                Console.WriteLine(s3);
                That(Angle.Parse(s3), Is.EqualTo(a.Reduce(WithNegative)).Within(1e-8));
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
                IsTrue(a.IsInDomain(AllPositive));
                IsTrue(a.Radians is <= TwoPi and >= 0, "{0} < Pi && {0} > -Pi", a);
            }

            {
                var a = new Angle(i * Pi / 10).Reduce(WithNegative);
                Console.WriteLine(a.ToString());
                IsTrue(a.IsInDomain(WithNegative));
                IsTrue(a.Radians is <= Pi and >= -Pi, "{0} < Pi && {0} > -Pi", a);
            }
        }
    }

    [Test]
    public void AngleEqualityTest()
    {
        IsFalse(new Angle(Pi).IsClose(new Angle(-Pi)));
        IsTrue(new Angle(Pi, AllPositive).IsClose(new Angle(-Pi, AllPositive)));
        IsTrue(new Angle(Pi, WithNegative).IsClose(new Angle(-Pi, WithNegative)));
    }

    [Test]
    public void AngleGreaterSmallerTest()
    {
        foreach (var i in Enumerable.Range(0, 6))
        {
            var a = new Angle(i);
            var a1 = a + HalfOfPi;
            var a2 = a - HalfOfPi;
            var a3 = a + Pi;
            var a4 = a + 0.1;
            var a5 = a - 0.1;
            var a6 = a - 0.9 * Pi;

            IsTrue(a1 > a);
            IsTrue(a2 < a);
            IsTrue(a4 > a);
            IsTrue(a5 < a);
            IsTrue(a6 < a);

            IsFalse(a1 < a);
            IsFalse(a2 > a);
            IsFalse(a4 < a);
            IsFalse(a5 > a);
            IsFalse(a6 > a);

            // ClassicAssert.IsFalse(a3 > a);
            IsFalse(a3 < a);
        }
    }

    [Test]
    public void VectorAdditionTest()
    {
        var ap1 = new AbsolutePoint(1, 1);
        var av1 = new AbsoluteVector(0, 1);
        That(new AbsolutePoint(1, 2).IsClose(ap1 + av1));

        var cp1 = new AbsolutePoint(1, 1);
        var cv1 = new AbsoluteVector(0, 1);
        That(new AbsolutePoint(1, 2).IsClose(cp1 + cv1));

        var pp1 = new PolarPoint(QuarterOfPi, 1);
        var pp2 = new PolarPoint(0, 1);
        var pv1 = new PolarVector(0, 1);
        var pv2 = new PolarVector(3 * QuarterOfPi, 1);
        That(new AbsolutePoint(1 + Sqrt2 / 2, Sqrt2 / 2).IsClose((pp1 + pv1).Absolute));
        That(new AbsolutePoint(0, Sqrt2).IsClose((pp1 + pv2).Absolute));
        That(new AbsolutePoint(1, Sqrt2).IsClose((pp1 + pv1 + pv2).Absolute));
        That(new AbsolutePoint(1, Sqrt2).IsClose((pp1 + (pv1 + pv2)).Absolute));

        That(new AbsolutePoint(2, 0).IsClose((pp2 + pv1).Absolute));
        That(new AbsolutePoint(0, 0).IsClose((pp2 + -pv1).Absolute));

        That((pv1 + pv2).IsClose(pp1 + pv1 + pv2 - pp1));
        That((pv1 + pv2).IsClose(pp1 + (pv1 + pv2) - pp1));
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

        foreach (var format in new[] { ":e3:deg", "V[]:f2:rad", "<;>::grad", "N{V,}:f2:gon" })
        {
            var s = pp.ToString(format, null);
            Console.WriteLine(s);
            IsTrue(PolarPoint.Parse(s).IsClose(pp, 2));

            s = pv.ToString(format, null);
            Console.WriteLine(s);
            IsTrue(PolarVector.Parse(s).IsClose(pv, 2));

            s = cp.ToString(format, null);
            Console.WriteLine(s);
            IsTrue(CartesianPoint.Parse(s).IsClose(cp, 2));

            s = cv.ToString(format, null);
            Console.WriteLine(s);
            IsTrue(CartesianVector.Parse(s).IsClose(cv, 2));

            s = ap.ToString(format, null);
            Console.WriteLine(s);
            IsTrue(AbsolutePoint.Parse(s).IsClose(ap, 2));

            s = av.ToString(format, null);
            Console.WriteLine(s);
            IsTrue(AbsoluteVector.Parse(s).IsClose(av, 2));
        }
    }

    [Test]
    public void VectorEnumerableTest()
    {
        var vectors = new[]
        {
            new PolarVector(0, 1),
            new PolarVector(HalfOfPi, 1),
            new PolarVector(Pi, 1),
            new PolarVector(ThreeHalfsOfPi, 2)
        };

        That(new PolarVector(-HalfOfPi, 1).IsClose(vectors.Sum()));

        var vectors2 = new[]
        {
            new PolarVector(0, Sqrt2),
            new PolarVector(HalfOfPi, Sqrt2),
            new PolarVector(QuarterOfPi, 1)
        };

        That(new PolarVector(QuarterOfPi, 1).IsClose(vectors2.Average()));
        That(new PolarVector(QuarterOfPi, 1).IsClose(vectors2.AverageDirection()));
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
        That(new AbsolutePoint(1, 1).IsClose(p1.Absolute));

        system.Origin = o1;
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        That(new AbsolutePoint(2, 2).IsClose(p1.Absolute));

        o1 = new AbsolutePoint(0, 0);
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        That(new AbsolutePoint(1, 1).IsClose(p1.Absolute), Is.False);

        // ReSharper disable once AccessToModifiedClosure
        system.OriginSource = () => o2;
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        That(new AbsolutePoint(0, 3).IsClose(p1.Absolute));

        o2 = new AbsolutePoint(0, 0);
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        That(new AbsolutePoint(1, 1).IsClose(p1.Absolute));

        var instance = new OriginClass()
        {
            Point = new AbsolutePoint(-1, -1)
        };
        system.OriginSource = () => instance.Point;
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        That(new AbsolutePoint(0, 0).IsClose(p1.Absolute));

        instance.Point = new AbsolutePoint(0, 0);
        Console.WriteLine(p1.Absolute.ToString("(,):f2", CultureInfo.InvariantCulture));
        That(new AbsolutePoint(1, 1).IsClose(p1.Absolute));
    }

    [Test]
    public void PolarCoordinates_AngleReduction()
    {
        var s1 = new PolarCoordinateSystem();
        var p1 = new PolarPoint(3 * Pi, 1, s1);
        Console.WriteLine($"p1:\t{p1}");
        IsFalse(p1.Phi.IsInDomain(AllPositive));
        IsFalse(p1.Phi.IsInDomain(WithNegative));
        AreEqual(3 * Pi, p1.Phi.Radians);

        var s2 = new PolarCoordinateSystem(angleReductionDomain: AllPositive);
        var p21 = new PolarPoint(3 * Pi, 1, s2);
        Console.WriteLine($"p21:\t{p21}");
        IsTrue(p21.Phi.IsInDomain(AllPositive));
        IsFalse(p21.Phi.IsInDomain(WithNegative));
        AreEqual(Pi, p21.Phi.Radians);
        var p22 = new PolarPoint(TwoPi, 1, s2);
        Console.WriteLine($"p22:\t{p22}");
        IsTrue(p22.Phi.IsInDomain(AllPositive));
        IsTrue(p22.Phi.IsInDomain(WithNegative));
        AreEqual(0, p22.Phi.Radians);

        var s3 = new PolarCoordinateSystem(angleReductionDomain: WithNegative);
        var p31 = new PolarPoint(1.5 * Pi, 1, s3);
        Console.WriteLine($"p31:\t{p31}");
        IsFalse(p31.Phi.IsInDomain(AllPositive));
        IsTrue(p31.Phi.IsInDomain(WithNegative));
        AreEqual(-0.5 * Pi, p31.Phi.Radians);
        var p32 = new PolarPoint(Pi, 1, s3);
        Console.WriteLine($"p32:\t{p32}");
        IsFalse(p32.Phi.IsInDomain(AllPositive));
        IsTrue(p32.Phi.IsInDomain(WithNegative));
        AreEqual(-Pi, p32.Phi.Radians);

        s3.AngleReductionDomain = AllPositive;
        Console.WriteLine($"p31:\t{p31}");
        IsTrue(p31.Phi.IsInDomain(AllPositive));
        IsFalse(p31.Phi.IsInDomain(WithNegative));
        AreEqual(1.5 * Pi, p31.Phi.Radians);
        Console.WriteLine($"p32:\t{p32}");
        IsTrue(p32.Phi.IsInDomain(AllPositive));
        IsFalse(p32.Phi.IsInDomain(WithNegative));
        AreEqual(Pi, p32.Phi.Radians);
    }
}