namespace RefraSin.MaterialData;

/// <summary>
/// Interface for types registering materials and material interfaces.
/// </summary>
public interface IMaterialRegistry : IReadOnlyMaterialRegistry
{
    /// <summary>
    /// Register a new material.
    /// </summary>
    /// <param name="material"></param>
    void RegisterMaterial(IMaterial material);

    /// <summary>
    /// Register a new material interface.
    /// </summary>
    /// <param name="materialInterface"></param>
    void RegisterMaterialInterface(IMaterialInterface materialInterface);
}
