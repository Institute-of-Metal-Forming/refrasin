using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.Compaction.ParticleModel;

internal interface IMoveableParticle : IMutableParticle<Node>
{
    public void MoveTowards(IMoveableParticle target, double distance) =>
        MoveTowards(target.Coordinates, distance);

    public void MoveTowards(IPoint target, double distance);

    public void MoveBy(IVector direction, double distance);

    public void Rotate(Angle angle);
}
