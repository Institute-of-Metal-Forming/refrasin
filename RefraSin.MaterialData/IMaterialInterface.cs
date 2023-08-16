namespace RefraSin.MaterialData;

public interface IMaterialInterface
{
    Guid From { get; }

    Guid To { get; }

    double InterfaceEnergy { get; }

    double DiffusionCoefficient { get; }

    double TransferCoefficient { get; }
}