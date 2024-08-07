namespace RefraSin.Coordinates;

public interface ICoordinateTransformations<out TCoordinates>
{
    public TCoordinates ScaleBy(double scale);

    public TCoordinates RotateBy(double rotation);
}
