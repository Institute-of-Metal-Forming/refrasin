using System.Net.Quic;
using RefraSin.Coordinates.Cartesian;

namespace RefraSin.ParquetStorage;

public class CartesianPointData
{
    public double X { get; set; }
    public double Y { get; set; }

    public static CartesianPointData From(ICartesianPoint from) => new() { X = from.X, Y = from.Y };
}
