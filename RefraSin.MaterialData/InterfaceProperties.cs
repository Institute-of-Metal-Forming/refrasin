namespace RefraSin.MaterialData;

public record InterfaceProperties(
    double DiffusionCoefficient,
    double Energy,
    double TransferCoefficient = 0
) : IInterfaceProperties { }
