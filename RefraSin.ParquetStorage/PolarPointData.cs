using RefraSin.Coordinates.Polar;

namespace RefraSin.ParquetStorage;

public class PolarPointData
{
    public double R { get; set; }
    public double Phi { get; set; }

    public static PolarPointData From(IPolarPoint from) =>
        new PolarPointData() { R = from.R, Phi = from.Phi };
}
