using RefraSin.Coordinates;
using RefraSin.ParticleModel;

namespace RefraSin.ParquetStorage;

public class NormalTangentialData
{
    public double Normal { get; set; }
    public double Tangential { get; set; }

    public static NormalTangentialData From(NormalTangential<double> from) =>
        new() { Normal = from.Normal, Tangential = from.Tangential };

    public static NormalTangentialData From(NormalTangential<Angle> from) =>
        new() { Normal = from.Normal, Tangential = from.Tangential };

    public NormalTangential<double> ToDoubleValued() => new(Normal, Tangential);

    public NormalTangential<Angle> ToAngleValued() => new(Normal, Tangential);
}
