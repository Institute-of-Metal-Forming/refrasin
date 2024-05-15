using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using static System.Double;

namespace RefraSin.TEPSolver.Normalization;

public interface INorm
{


    public double Time { get; }
    public double Mass { get; }
    public double Length { get; }
    public double Temperature { get; }
    public double Substance { get; }

    public double Area { get; }
    public double Volume { get; }
    public double Energy { get; }
    public double DiffusionCoefficient { get; }
    public double InterfaceEnergy { get; }
}