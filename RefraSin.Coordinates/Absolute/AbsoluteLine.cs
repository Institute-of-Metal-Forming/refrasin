using System.Globalization;

namespace RefraSin.Coordinates.Absolute;

public class AbsoluteLine : ILine, IFormattable
{
    public AbsoluteLine(IPoint referencePoint, IVector directionVector)
    {
        ReferencePoint = referencePoint.Absolute;
        DirectionVector = directionVector.Absolute;

        A = -DirectionVector.Y;
        B = DirectionVector.X;
        C =
            (ReferencePoint.X + DirectionVector.X) * ReferencePoint.Y
            - ReferencePoint.X * (ReferencePoint.Y + DirectionVector.Y);
    }

    public AbsoluteLine(IPoint referencePoint, IPoint secondPoint)
        : this(referencePoint, referencePoint.Absolute.VectorTo(secondPoint.Absolute)) { }

    public AbsolutePoint ReferencePoint { get; }

    IPoint ILine.ReferencePoint => ReferencePoint;

    public AbsoluteVector DirectionVector { get; }

    public double A { get; }

    public double B { get; }

    public double C { get; }

    IVector ILine.DirectionVector => DirectionVector;

    public AbsolutePoint IntersectionTo(ILine other)
    {
        var otherAbs =
            other as AbsoluteLine ?? new AbsoluteLine(other.ReferencePoint, other.DirectionVector);

        var denominator = A * otherAbs.B - otherAbs.A * B;

        if (denominator == 0)
            throw new InvalidOperationException("lines are parallel, there is no intersection");

        var x = (C * otherAbs.B - otherAbs.C * B) / denominator;
        var y = (A * otherAbs.C - otherAbs.A * C) / denominator;

        return new AbsolutePoint(x, y);
    }

    IPoint ILine.IntersectionTo(ILine other) => IntersectionTo(other);

    public AbsolutePoint EvaluateAtParameter(double parameter) =>
        ReferencePoint + parameter * DirectionVector;

    IPoint ILine.EvaluateAtParameter(double parameter) => EvaluateAtParameter(parameter);

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var formats = format?.Split(':');
        var coordinateFormat = formats?[0];
        var numberFormat = formats?.Length > 1 ? formats[1] : null;
        
        return 
            $"{nameof(AbsoluteLine)} @ {ReferencePoint.ToString(format, formatProvider)} -> {DirectionVector.ToString(format, formatProvider)} / {A.ToString(numberFormat, formatProvider)}x + {B.ToString(numberFormat,formatProvider)}y = {C.ToString(numberFormat,formatProvider)} ";
    }

    /// <inheritdoc />
    public override string ToString() => ToString("(,)", CultureInfo.InvariantCulture);
}
