using RefraSin.Coordinates;

namespace RefraSin.ParticleModel.Particles;

public interface IParticleShifts
{
    double RadialShift { get; }

    Angle AngleShift { get; }

    Angle RotationShift { get; }
}
