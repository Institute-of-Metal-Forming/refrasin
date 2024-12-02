using RefraSin.Coordinates;
using RefraSin.ParticleModel;

namespace RefraSin.ParquetStorage;

public class ToUpperToLowerData
{
    public double ToUpper { get; set; }
    public double ToLower { get; set; }

    public static ToUpperToLowerData From(ToUpperToLower<double> from) =>
        new() { ToUpper = from.ToUpper, ToLower = from.ToLower };

    public static ToUpperToLowerData From(ToUpperToLower<Angle> from) =>
        new() { ToUpper = from.ToUpper, ToLower = from.ToLower };

    public ToUpperToLower<double> ToDoubleValued() => new(ToUpper, ToLower);

    public ToUpperToLower<Angle> ToAngleValued() => new(ToUpper, ToLower);
}
