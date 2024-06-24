namespace RefraSin.MaterialData;

public interface IInterfaceProperties
{
    /// <summary>
    /// Diffusion coefficient along interface.
    /// </summary>
    double DiffusionCoefficient { get; }

    /// <summary>
    /// Interface energy.
    /// </summary>
    double Energy { get; }
}