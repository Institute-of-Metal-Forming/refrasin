namespace RefraSin.MaterialData;

public interface IInterfaceProperties
{
    /// <summary>
    /// Diffusion coefficient along interface (including vacancy concentration).
    /// </summary>
    double DiffusionCoefficient { get; }

    /// <summary>
    /// Interface energy.
    /// </summary>
    double Energy { get; }

    /// <summary>
    /// Transfer (diffusion) coefficient cross the interface (including vacancy concentration).
    /// </summary>
    double TransferCoefficient { get; }
}
