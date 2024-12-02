using RefraSin.Coordinates;
using RefraSin.Coordinates.Cartesian;
using RefraSin.MaterialData;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ParquetStorage;

public class ParticleData : IParticle
{
    public Guid Id { get; set; }
    public double RotationAngle { get; set; }
    Angle ICoordinateSystem.RotationAngle => RotationAngle;
    public CartesianPointData Coordinates { get; set; }
    ICartesianPoint IParticle.Coordinates => new CartesianPoint(Coordinates.X, Coordinates.Y);
    public Guid MaterialId { get; set; }

    public static ParticleData From(IParticle particle) =>
        new()
        {
            Id = particle.Id,
            RotationAngle = particle.RotationAngle,
            Coordinates = CartesianPointData.From(particle.Coordinates),
        };
}
