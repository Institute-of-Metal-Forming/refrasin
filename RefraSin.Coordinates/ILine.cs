namespace RefraSin.Coordinates;

public interface ILine
{
    IPoint ReferencePoint { get; }

    IVector DirectionVector { get; }

    IPoint IntersectionTo(ILine other);

    IPoint EvaluateAtParameter(double parameter);
}
