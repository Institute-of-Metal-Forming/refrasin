namespace RefraSin.MaterialData;

/// <summary>
/// Interface for types registering materials and material interfaces.
/// </summary>
public interface IMaterialRegistry
{
    /// <summary>
    /// List of registered materials.
    /// </summary>
    IReadOnlyList<IMaterial> Materials { get; }

    /// <summary>
    /// List of registered material interfaces.
    /// </summary>
    IReadOnlyList<IMaterialInterface> MaterialInterfaces { get; }

    /// <summary>
    /// Register a new material.
    /// </summary>
    /// <param name="material"></param>
    void RegisterMaterial(IMaterial material);

    /// <summary>
    /// Get a registered material by its ID.
    /// </summary>
    /// <param name="id"></param>
    IMaterial GetMaterial(Guid id);

    /// <summary>
    /// Get a registered material by its name.
    /// </summary>
    /// <param name="name"></param>
    IMaterial GetMaterial(string name);

    /// <summary>
    /// Register a new material interface.
    /// </summary>
    /// <param name="materialInterface"></param>
    void RegisterMaterialInterface(IMaterialInterface materialInterface);

    /// <summary>
    /// Get a material interface by the IDs of the involved materials.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    IMaterialInterface GetMaterialInterface(Guid from, Guid to);

    /// <summary>
    /// Get a material interface by giving the instances of the involved materials. Relies on the IDs in code behind.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    IMaterialInterface GetMaterialInterface(IMaterial from, IMaterial to) => GetMaterialInterface(from.Id, to.Id);

    /// <summary>
    /// Get a material interface by the names of the involved materials.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    IMaterialInterface GetMaterialInterface(string from, string to);
}