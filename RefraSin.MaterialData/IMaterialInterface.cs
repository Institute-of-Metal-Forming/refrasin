namespace RefraSin.MaterialData;

/// <summary>
/// Interface for types providing material data of interfaces (boundaries, ...).
/// </summary>
public interface IMaterialInterface
{
    /// <summary>
    /// Unique ID of the material the view is originating from.
    /// </summary>
    Guid From { get; }

    /// <summary>
    /// Unique ID of the material the view is directing to.
    /// </summary>
    Guid To { get; }

    /// <summary>
    /// Interface energy (supposed to be equal in both view directions).
    /// </summary>
    double InterfaceEnergy { get; }

    /// <summary>
    /// Diffusion coefficient of From's atoms along the interface.
    /// </summary>
    double DiffusionCoefficient { get; }

    /// <summary>
    /// Transfer coefficient of From's atoms cross the interface.
    /// </summary>
    double TransferCoefficient { get; }
}