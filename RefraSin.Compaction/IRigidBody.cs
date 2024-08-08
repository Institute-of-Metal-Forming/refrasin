using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;

namespace RefraSin.Compaction;

internal interface IRigidBody
{
    AbsolutePoint Coordinates { get; }

    Angle RotationAngle { get; }

    public void MoveTowards(IRigidBody target, double distance) =>
        MoveTowards(target.Coordinates, distance);

    public void MoveTowards(IPoint target, double distance);

    public void Rotate(Angle angle);
}
